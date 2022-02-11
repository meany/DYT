using dm.DYT.Common;
using dm.DYT.Data;
using dm.DYT.Data.Models;
using dm.DYT.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace dm.DYT.Stats
{
    class Program
    {
        private IServiceProvider services;
        private IConfigurationRoot configuration;
        private Config config;
        private AppDbContext db;
        private static Logger log = LogManager.GetCurrentClassLogger();

        private BigInteger supply;
        private BigInteger burned;
        private List<EsTxsResult> esTxs;
        private List<EsInternalTxsResult> esInternalTxs;
        private List<Transaction> dbTxs;

        public static void Main(string[] args)
            => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Config.Stats.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("Config.Stats.Local.json", optional: true, reloadOnChange: true);

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
                log.Info("Getting Etherscan info");
                await GetInfo();
                await InsertNewTxs();
                GetInternalTxInfo();
                await InsertNewInternalTxs();

                dbTxs = db.Transactions
                    .AsNoTracking()
                    .ToList();
                int totalTxs = (dbTxs.Count - 1) / 2;

                GetBurned();
                var burn1 = GetBurnedHours(1);
                var burn24 = GetBurnedHours(24);
                var burnAvg = GetBurnedAverage();

                var circulation = supply;

                var item = new Stat
                {
                    Burned = burned.ToEth(),
                    BurnLast1H = burn1.ToEth(),
                    BurnLast24H = burn24.ToEth(),
                    BurnAvgDay = burnAvg.ToEth(),
                    Circulation = circulation.ToEth(),
                    Date = DateTime.UtcNow,
                    Group = Guid.NewGuid(),
                    Supply = supply.ToEth(),
                    Transactions = totalTxs
                };

                db.Add(item);

                log.Info("Saving stats to database");
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async Task GetInfo()
        {
            try
            {
                var client = new RestClient("https://api.etherscan.io");
                GetTxs(client);
                await Task.Delay(200);
                GetSupply(client);

                while (supply == 0 || esTxs == null)
                {
                    await Task.Delay(200);
                    //log.Debug($"supply: {supply} | esTxs: {esTxs == null}");
                }

                client = null;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void GetInternalTxInfo()
        {
            try
            {
                var client = new RestClient("https://api.etherscan.io");
                GetInternalTxs(client);
                client = null;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void GetSupply(RestClient client)
        {
            var req = new RestRequest("api", Method.GET);
            new RestRequest("getTokenInfo/0x740623d2c797b7d8d1ecb98e9b4afcf99ec31e14", Method.GET);
            req.AddParameter("time", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            req.AddParameter("module", "stats");
            req.AddParameter("action", "tokensupply");
            req.AddParameter("contractaddress", "0x740623d2c797b7d8d1ecb98e9b4afcf99ec31e14");
            req.AddParameter("apikey", config.EtherscanToken);
            client.ExecuteAsync<EsToken>(req, res =>
            {
                supply = BigInteger.Parse(res.Data.Result);
                log.Info($"GetSupply: OK ({supply})");
            });
        }

        private void GetTxs(RestClient client)
        {
            var lastTx = db.Transactions
                .AsNoTracking()
                .OrderByDescending(x => x.TimeStamp)
                .FirstOrDefault();

            int start = 0;
            if (lastTx != null)
                start = int.Parse(lastTx.BlockNumber) + 1;

            var req = new RestRequest("api", Method.GET);
            req.AddParameter("time", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            req.AddParameter("module", "account");
            req.AddParameter("action", "tokentx");
            req.AddParameter("contractaddress", "0x740623d2c797b7d8d1ecb98e9b4afcf99ec31e14");
            req.AddParameter("startblock", start);
            req.AddParameter("endblock", "999999999");
            req.AddParameter("apikey", config.EtherscanToken);
            client.ExecuteAsync<EsTxs>(req, res =>
            {
                if (res.Data.Result.Count == 0)
                {
                    esTxs = new List<EsTxsResult>();
                    log.Info($"GetTxs: {res.Data.Message} (0)");
                    return;
                }

                esTxs = res.Data.Result
                    .OrderBy(x => x.BlockNumber)
                    .ToList();
                log.Info($"GetTxs: {res.Data.Message} ({esTxs.Count()}: {start} to {esTxs.Last().BlockNumber})");
            });
        }

        private void GetInternalTxs(RestClient client)
        {
            if (esInternalTxs == null)
            {
                esInternalTxs = new List<EsInternalTxsResult>();
            }

            foreach (var tx in esTxs)
            {
                if (tx.From == config.UniswapDYTAddress || tx.To == config.UniswapDYTAddress)
                {

                    var req = new RestRequest("api", Method.GET);
                    req.AddParameter("time", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                    req.AddParameter("module", "account");
                    req.AddParameter("action", "txlistinternal");
                    req.AddParameter("txhash", tx.Hash);
                    req.AddParameter("apikey", config.EtherscanToken);
                    var handle = client.ExecuteAsync<EsInternalTxs>(req, res =>
                    {
                        if (res.Data.Result.Count == 0 || res.Data.Result.Count > 2)
                        {
                            log.Info($"GetInternalTxs: {res.Data.Message}");
                            return;
                        }

                        esInternalTxs.Add(res.Data.Result
                            .Where(x => x.From == config.UniswapWETHAddress ||
                                x.To == config.UniswapWETHAddress)
                            .First());
                        log.Info($"GetInternalTxs: {res.Data.Message} ({esInternalTxs.Count})");
                    });
                }
            }
        }

        private async Task InsertNewTxs()
        {
            if (esTxs.Count > 0)
            {
                log.Info("Inserting newest transactions");
                foreach (var tx in esTxs)
                {
                    var newTx = new Transaction
                    {
                        BlockNumber = tx.BlockNumber,
                        Hash = tx.Hash,
                        From = tx.From,
                        To = tx.To,
                        TimeStamp = tx.TimeStamp,
                        Value = tx.Value
                    };
                    db.Add(newTx);
                }
                await db.SaveChangesAsync();
            }
        }

        private async Task InsertNewInternalTxs()
        {
            if (esInternalTxs.Count > 0)
            {
                log.Info("Inserting newest Uniswap transactions");
                var prices = await Data.Common.GetPrices(db);

                foreach (var intTx in esInternalTxs)
                {
                    var tx = db.Transactions.First(x => x.BlockNumber == intTx.BlockNumber);
                    var valDYT = BigInteger.Parse(tx.Value).ToEth();
                    var valWETH = BigInteger.Parse(intTx.Value).ToEth();
                    var valUSD = valDYT * prices.PriceUSD;

                    var newUniTx = new UniswapTransaction
                    {
                        Transaction = tx,
                        TxType = (intTx.From == config.UniswapDYTAddress)
                            ? UniswapTransactionType.Buy : UniswapTransactionType.Sell,
                        DYT = valDYT,
                        WETH = valWETH,
                        USD = valUSD
                    };
                    
                    db.Add(newUniTx);

                    // start telegram bot w/ tx
                }
            }
        }

        private void GetBurned()
        {
            var total = BigInteger.Parse("2000000000000000000000000");
            burned = BigInteger.Subtract(total, supply);
        }

        private BigInteger GetBurnedHours(int hours)
        {
            var dt = DateTime.UtcNow.AddHours(-hours);
            var txs1 = dbTxs.Where(x => x.TimeStamp >= dt &&
                x.To == "0x0000000000000000000000000000000000000000")
                .Select(x => BigInteger.Parse(x.Value));

            var bi = new BigInteger();
            foreach (var tx1 in txs1)
            {
                bi = bi + tx1;
            }
            return bi;
        }

        private BigInteger GetBurnedAverage()
        {
            var first = dbTxs.OrderBy(x => x.TimeStamp).First();
            var days = Math.Ceiling((DateTime.UtcNow - first.TimeStamp).TotalDays);
            var txs1 = dbTxs.Where(x => x.To == "0x0000000000000000000000000000000000000000")
                .Select(x => BigInteger.Parse(x.Value));

            var bi = new BigInteger();
            foreach (var tx1 in txs1)
            {
                bi = bi + tx1;
            }

            var avg = BigInteger.Divide(bi, BigInteger.Parse(days.ToString()));
            return avg;
        }
    }
}