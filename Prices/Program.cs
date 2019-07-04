using dm.DYT.Data;
using dm.DYT.Data.Models;
using dm.DYT.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using PureWebSockets;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace dm.DYT.Prices
{
    class Program
    {
        private IServiceProvider services;
        private IConfigurationRoot configuration;
        private Config config;
        private AppDbContext db;
        private static Logger log = LogManager.GetCurrentClassLogger();

        private EsPriceResult esPrice;
        private ManualResetEvent mreMarket = new ManualResetEvent(false);
        private PureWebSocket ws;
        private decimal mktPriceFdEth;
        private decimal mktVolumeFdEth;

        private BrTicker brTicker;
        private BrStats brStats;
        private decimal mktPriceBrEth;
        private decimal mktVolumeBrEth;

        public static void Main(string[] args)
            => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Config.Prices.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("Config.Prices.Local.json", optional: true, reloadOnChange: true);

                configuration = builder.Build();

                services = new ServiceCollection()
                    .Configure<Config>(configuration)
                    .AddDatabase<AppDbContext>(configuration.GetConnectionString("Database"))
                    .BuildServiceProvider();
                config = services.GetService<IOptions<Config>>().Value;
                db = services.GetService<AppDbContext>();

                db.Database.Migrate();

                Start();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async void Start()
        {
            try
            {
                log.Info("Getting info");
                var stat = db.Stats
                    .AsNoTracking()
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefault();

                await GetInfo();

                // market data: ForkDelta
                decimal priceBtcFd = mktPriceFdEth * decimal.Parse(esPrice.EthBtc);
                decimal priceUsdFd = mktPriceFdEth * decimal.Parse(esPrice.EthUsd);
                int mktCapUsdFd = (int)Math.Round(priceUsdFd * stat.Circulation);
                int volumeUsdFd = (int)Math.Round(decimal.Parse(esPrice.EthUsd) * mktVolumeFdEth);

                // market data: BambooRelay
                decimal priceBtcBr = mktPriceBrEth * decimal.Parse(esPrice.EthBtc);
                decimal priceUsdBr = mktPriceBrEth * decimal.Parse(esPrice.EthUsd);
                int mktCapUsdBr = (int)Math.Round(priceUsdBr * stat.Circulation);
                int volumeUsdBr = (int)Math.Round(decimal.Parse(esPrice.EthUsd) * mktVolumeBrEth);

                // totals
                int volumeTotal = volumeUsdFd + volumeUsdBr;
                decimal volumePctFd = (decimal)volumeUsdFd / volumeTotal;
                decimal volumePctBr = (decimal)volumeUsdBr / volumeTotal;

                int mktCapTotal = mktCapUsdFd + mktCapUsdBr;
                int mktCapWtdFd = (int)Math.Round(mktCapUsdFd * volumePctFd);
                int mktCapWtdBr = (int)Math.Round(mktCapUsdBr * volumePctBr);
                
                // prices
                decimal priceUsdWtdFd = priceUsdFd * volumePctFd;
                decimal priceUsdWtdBr = priceUsdBr * volumePctBr;
                decimal priceEthWtdFd = mktPriceFdEth * volumePctFd;
                decimal priceEthWtdBr = mktPriceBrEth * volumePctBr;
                decimal priceBtcWtdFd = priceBtcFd * volumePctFd;
                decimal priceBtcWtdBr = priceBtcBr * volumePctBr;

                var item1 = new Price
                {
                    Base = PriceBase.Ethereum,
                    Date = DateTime.UtcNow,
                    Group = stat.Group,
                    MarketCapUSD = mktCapUsdFd,
                    MarketCapUSDWeighted = mktCapWtdFd,
                    PriceBTC = priceBtcFd,
                    PriceBTCWeighted = priceBtcWtdFd,
                    PriceETH = mktPriceFdEth,
                    PriceETHWeighted = priceEthWtdFd,
                    PriceUSD = priceUsdFd,
                    PriceUSDWeighted = priceUsdWtdFd,
                    Source = PriceSource.ForkDelta,
                    VolumeUSD = volumeUsdFd,
                    VolumeUSDPct = volumePctFd,
                };

                db.Add(item1);

                var item2 = new Price
                {
                    Base = PriceBase.Ethereum,
                    Date = DateTime.UtcNow,
                    Group = stat.Group,
                    MarketCapUSD = mktCapUsdBr,
                    MarketCapUSDWeighted = mktCapWtdBr,
                    PriceBTC = priceBtcBr,
                    PriceBTCWeighted = priceBtcWtdBr,
                    PriceETH = mktPriceBrEth,
                    PriceETHWeighted = priceEthWtdBr,
                    PriceUSD = priceUsdBr,
                    PriceUSDWeighted = priceUsdWtdBr,
                    Source = PriceSource.BambooRelay,
                    VolumeUSD = volumeUsdBr,
                    VolumeUSDPct = volumePctBr,
                };

                db.Add(item2);

                log.Info("Saving prices to database");
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private Task GetInfo()
        {
            try
            {
                var client = new RestClient("https://api.etherscan.io");
                GetPrices(client);

                GetForkDelta();

                var client2 = new RestClient("https://rest.bamboorelay.com");
                GetBambooRelay(client2);

                while (esPrice == null || brTicker == null || brStats == null)
                {
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return Task.CompletedTask;
        }

        private void GetPrices(RestClient client)
        {
            var req = new RestRequest("api", Method.GET);
            req.AddParameter("module", "stats");
            req.AddParameter("action", "ethprice");
            req.AddParameter("apikey", config.EtherscanToken);
            client.ExecuteAsync<EsPrice>(req, res =>
            {
                esPrice = res.Data.Result;
                log.Info($"GetPrices: {res.Data.Message}");
            });
        }

        private void GetForkDelta()
        {
            var opts = new PureWebSocketOptions
            {
                SendCacheItemTimeout = TimeSpan.FromSeconds(60)
            };
            ws = new PureWebSocket("wss://api.forkdelta.app/socket.io/?EIO=3&transport=websocket", opts);
            ws.OnMessage += Ws_OnMessage;
            ws.Connect();

            mreMarket.WaitOne();
            ws.Dispose();
        }

        private void Ws_OnMessage(string message)
        {
            if (message == "40")
            {
                ws.Send("42[\"getMarket\",{\"token\":\"0xad95a3c0fdc9bc4b27fd79e028a0a808d5564aa4\"}]");
            }
            else if (message.StartsWith("42[\"market\","))
            {
                var subMsg = message.Substring(12).TrimEnd(']');
                var market = JsonConvert.DeserializeObject<FdMarket>(subMsg);

                var lastTrade = market.Trades
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefault();
                mktPriceFdEth = decimal.Parse(lastTrade.Price);

                mktVolumeFdEth = market.Trades
                    .Where(x => x.Date >= DateTime.UtcNow.AddDays(-1))
                    .Sum(x => decimal.Parse(x.AmountBase, NumberStyles.Float));

                log.Info($"GetForkDelta: OK");
                mreMarket.Set();
            }
        }

        private void GetBambooRelay(RestClient client)
        {
            var req = new RestRequest("main/0x/markets/DYT-WETH/stats", Method.GET);
            req.AddParameter("include", "ticker");
            client.ExecuteAsync<BrResult>(req, res =>
            {
                brStats = res.Data.Stats;
                brTicker = res.Data.Ticker;
                mktVolumeBrEth = decimal.Parse(brStats.Volume24Hour);
                mktPriceBrEth = decimal.Parse(brTicker.Price);
            });

            log.Info($"GetBambooRelay: OK");
        }
    }
}