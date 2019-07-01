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
        private decimal mktPrice;
        private decimal mktVolume;

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
                log.Info("Getting Etherscan info");
                await GetInfo();
                log.Info("Getting market data");
                GetMarketDataAsync();

                var stat = db.Stats
                    .AsNoTracking()
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefault();

                decimal priceBtc = mktPrice * decimal.Parse(esPrice.EthBtc);
                decimal priceUsd = mktPrice * decimal.Parse(esPrice.EthUsd);
                int mktCapUsd = int.Parse(Math.Round(priceUsd * stat.Circulation).ToString());
                int volumeUsd = int.Parse(Math.Round(decimal.Parse(esPrice.EthUsd) * mktVolume).ToString());

                var item = new Price
                {
                    Date = DateTime.UtcNow,
                    MarketCapUSD = mktCapUsd,
                    PriceBTC = priceBtc,
                    PriceETH = mktPrice,
                    PriceUSD = priceUsd,
                    VolumeUSD = volumeUsd
                };

                db.Add(item);

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
                Thread.Sleep(200);

                while (esPrice == null)
                {
                    Thread.Sleep(1000);
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

        private void GetMarketDataAsync()
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
                mktPrice = decimal.Parse(lastTrade.Price);

                mktVolume = market.Trades
                    .Where(x => x.Date >= DateTime.UtcNow.AddDays(-1))
                    .Sum(x => decimal.Parse(x.AmountBase, NumberStyles.Float));

                mreMarket.Set();
            }
        }
    }
}