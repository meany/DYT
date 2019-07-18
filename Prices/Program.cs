using CoinGecko.Clients;
using CoinGecko.Entities.Response.Coins;
using dm.DYT.Data;
using dm.DYT.Data.Models;
using dm.DYT.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NLog;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
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

        private CoinFullDataById data;

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

                await Start();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async Task Start()
        {
            try
            {
                log.Info("Getting info");
                var stat = db.Stats
                    .AsNoTracking()
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefault();

                await GetInfo();

                // market cap
                int mktCapUsd = int.Parse(data.MarketData.MarketCap["usd"].Value.ToString());
                decimal mktCapUsdChgAmt = decimal.Parse(data.MarketData.MarketCapChange24HInCurrency["usd"].ToString(), NumberStyles.Any);
                Change mktCapUsdChg = (mktCapUsdChgAmt > 0) ? Change.Up : (mktCapUsdChgAmt < 0) ? Change.Down : Change.None;
                decimal mktCapUsdChgPct = decimal.Parse(data.MarketData.MarketCapChangePercentage24HInCurrency["usd"].ToString(), NumberStyles.Any);

                // volume
                int volumeUsd = (int)Math.Round(data.MarketData.TotalVolume["usd"].Value);

                // prices
                decimal priceBtc = decimal.Parse(data.MarketData.CurrentPrice["btc"].Value.ToString(), NumberStyles.Any);
                decimal priceBtcChgAmt = decimal.Parse(data.MarketData.PriceChange24HInCurrency["btc"].ToString(), NumberStyles.Any);
                Change priceBtcChg = (priceBtcChgAmt > 0) ? Change.Up : (priceBtcChgAmt < 0) ? Change.Down : Change.None;
                decimal priceBtcChgPct = decimal.Parse(data.MarketData.PriceChangePercentage24HInCurrency["btc"].ToString(), NumberStyles.Any);

                decimal priceEth = decimal.Parse(data.MarketData.CurrentPrice["eth"].Value.ToString(), NumberStyles.Any);
                decimal priceEthChgAmt = decimal.Parse(data.MarketData.PriceChange24HInCurrency["eth"].ToString(), NumberStyles.Any);
                Change priceEthChg = (priceEthChgAmt > 0) ? Change.Up : (priceEthChgAmt < 0) ? Change.Down : Change.None;
                decimal priceEthChgPct = decimal.Parse(data.MarketData.PriceChangePercentage24HInCurrency["eth"].ToString(), NumberStyles.Any);

                decimal priceUsd = decimal.Parse(data.MarketData.CurrentPrice["usd"].Value.ToString(), NumberStyles.Any);
                decimal priceUsdChgAmt = decimal.Parse(data.MarketData.PriceChange24HInCurrency["usd"].ToString(), NumberStyles.Any);
                Change priceUsdChg = (priceUsdChgAmt > 0) ? Change.Up : (priceUsdChgAmt < 0) ? Change.Down : Change.None;
                decimal priceUsdChgPct = decimal.Parse(data.MarketData.PriceChangePercentage24HInCurrency["usd"].ToString(), NumberStyles.Any);

                var item = new Price360
                {
                    Date = DateTime.UtcNow,
                    Group = stat.Group,
                    MarketCapUSD = mktCapUsd,
                    MarketCapUSDChange = mktCapUsdChg,
                    MarketCapUSDChangePct = mktCapUsdChgPct,
                    PriceBTC = priceBtc,
                    PriceBTCChange = priceBtcChg,
                    PriceBTCChangePct = priceBtcChgPct,
                    PriceETH = priceEth,
                    PriceETHChange = priceEthChg,
                    PriceETHChangePct = priceEthChgPct,
                    PriceUSD = priceUsd,
                    PriceUSDChange = priceUsdChg,
                    PriceUSDChangePct = priceUsdChgPct,
                    VolumeUSD = volumeUsd,
                    Source = Price360Source.CoinGecko,
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
                GetPrices();

                while (data == null)
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

        private async void GetPrices()
        {
            var client = CoinGeckoClient.Instance;
            data = await client.CoinsClient.GetAllCoinDataWithId("dynamite", "false", true, true, false, false, false);

            log.Info($"GetPrices: OK");
        }
    }
}