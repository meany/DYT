using dm.DYT.Common;
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
        private BigInteger fund;
        private BigInteger fund2;
        private BigInteger fund3;
        private List<EsTxsResult> esTxs;
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
                log.Info("Inserting newest transactions");
                InsertNewTxs();

                dbTxs = await db.Transactions
                    .AsNoTracking()
                    .ToListAsync()
                    .ConfigureAwait(false);
                int totalTxs = (dbTxs.Count - 1) / 2;

                var totalBurned = GetBurned();
                var burn1 = GetBurnedHours(1);
                var burn24 = GetBurnedHours(24);
                var burnAvg = GetBurnedAverage();
                var circulation = GetCirculation();

                var item = new Stat
                {
                    Burned = totalBurned.ToEth(),
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

        private Task GetInfo()
        {
            try
            {
                var client = new RestClient("https://api.etherscan.io");
                GetTxs(client);
                Thread.Sleep(200);
                GetSupply(client);
                Thread.Sleep(200);
                GetFund(client);
                Thread.Sleep(200);
                GetFund2(client);
                Thread.Sleep(200);
                GetFund3(client);

                while (supply == null || fund == null || fund2 == null || fund3 == null || esTxs == null)
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

        private void GetSupply(RestClient client)
        {
            var req = new RestRequest("api", Method.GET);
            req.AddParameter("module", "stats");
            req.AddParameter("action", "tokensupply");
            req.AddParameter("contractaddress", "0xad95a3c0fdc9bc4b27fd79e028a0a808d5564aa4");
            req.AddParameter("apikey", config.EtherscanToken);
            client.ExecuteAsync<EsToken>(req, res =>
            {
                supply = BigInteger.Parse(res.Data.Result);
                log.Info($"GetSupply: {res.Data.Message}");
            });
        }

        private void GetFund(RestClient client)
        {
            var req = new RestRequest("api", Method.GET);
            req.AddParameter("module", "account");
            req.AddParameter("action", "tokenbalance");
            req.AddParameter("contractaddress", "0xad95a3c0fdc9bc4b27fd79e028a0a808d5564aa4");
            req.AddParameter("address", "0x4a08a3cbc29dea9a9d7ee448ef754376b4983763");
            req.AddParameter("apikey", config.EtherscanToken);
            client.ExecuteAsync<EsToken>(req, res =>
            {
                fund = BigInteger.Parse(res.Data.Result);
                log.Info($"GetFund: {res.Data.Message}");
            });
        }

        private void GetFund2(RestClient client)
        {
            var req = new RestRequest("api", Method.GET);
            req.AddParameter("module", "account");
            req.AddParameter("action", "tokenbalance");
            req.AddParameter("contractaddress", "0xad95a3c0fdc9bc4b27fd79e028a0a808d5564aa4");
            req.AddParameter("address", "0x26bb02501e8460c4eb461df0e9a7d598b9aef190");
            req.AddParameter("apikey", config.EtherscanToken);
            client.ExecuteAsync<EsToken>(req, res =>
            {
                fund2 = BigInteger.Parse(res.Data.Result);
                log.Info($"GetFund2: {res.Data.Message}");
            });
        }

        private void GetFund3(RestClient client)
        {
            var req = new RestRequest("api", Method.GET);
            req.AddParameter("module", "account");
            req.AddParameter("action", "tokenbalance");
            req.AddParameter("contractaddress", "0xad95a3c0fdc9bc4b27fd79e028a0a808d5564aa4");
            req.AddParameter("address", "0x010b3a9e0a199e45ebb3d08de47479ffe6a789a1");
            req.AddParameter("apikey", config.EtherscanToken);
            client.ExecuteAsync<EsToken>(req, res =>
            {
                fund3 = BigInteger.Parse(res.Data.Result);
                log.Info($"GetFund3: {res.Data.Message}");
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
            req.AddParameter("module", "account");
            req.AddParameter("action", "tokentx");
            req.AddParameter("contractaddress", "0xad95a3c0fdc9bc4b27fd79e028a0a808d5564aa4");
            req.AddParameter("startblock", start);
            req.AddParameter("endblock", "999999999");
            req.AddParameter("apikey", config.EtherscanToken);
            client.ExecuteAsync<EsTxs>(req, res =>
            {
                esTxs = res.Data.Result
                    .OrderBy(x => x.BlockNumber)
                    .ToList();
                log.Info($"GetTxs: {res.Data.Message}");
            });
        }

        private void InsertNewTxs()
        {
            foreach (var tx in esTxs)
            {
                var newTx = new Transaction
                {
                    BlockNumber = tx.BlockNumber,
                    Hash = tx.Hash,
                    To = tx.To,
                    TimeStamp = tx.TimeStamp,
                    Value = tx.Value
                };
                db.Add(newTx);
            }
            db.SaveChanges();
        }

        private BigInteger GetBurned()
        {
            var total = BigInteger.Parse("2000000000000000000000000");
            return BigInteger.Subtract(total, supply);
        }

        private BigInteger GetCirculation()
        {
            var total = BigInteger.Parse("2000000000000000000000000");
            total = BigInteger.Subtract(total, fund);
            total = BigInteger.Subtract(total, fund2);
            total = BigInteger.Subtract(total, fund3);
            return total;
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