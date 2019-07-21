using dm.DYT.Common;
using dm.DYT.Data;
using dm.DYT.Data.Models;
using dm.DYT.Response;
using Microsoft.EntityFrameworkCore;
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
                log.Info("Getting Etherscan/Ethplorer info");
                await GetInfo();
                await InsertNewTxs();

                dbTxs = db.Transactions
                    .AsNoTracking()
                    .ToList();
                int totalTxs = (dbTxs.Count - 1) / 2;

                GetBurned();
                var burn1 = GetBurnedHours(1);
                var burn24 = GetBurnedHours(24);
                var burnAvg = GetBurnedAverage();

                var circulation = GetCirculation();

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

                var client2 = new RestClient("https://api.ethplorer.io");
                GetSupply(client2);
                await Task.Delay(200);
                GetFund(client2);
                await Task.Delay(200);
                GetFund2(client2);
                await Task.Delay(200);
                GetFund3(client2);

                while (supply == 0 || fund == 0 || fund2 == 0 || fund3 == 0 || esTxs == null)
                {
                    await Task.Delay(200);
                }

                client = null;
                client2 = null;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void GetSupply(RestClient client)
        {
            var req = new RestRequest("getTokenInfo/0xad95a3c0fdc9bc4b27fd79e028a0a808d5564aa4", Method.GET);
            req.AddParameter("time", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            req.AddParameter("apiKey", "freekey");
            client.ExecuteAsync<EpToken>(req, res =>
            {
                supply = BigInteger.Parse(res.Data.TotalSupply);
                log.Info($"GetSupply: OK ({supply.ToString()})");
            });
        }

        private void GetFund(RestClient client)
        {
            var req = new RestRequest("getAddressInfo/0x4a08a3cbc29dea9a9d7ee448ef754376b4983763", Method.GET);
            req.AddParameter("time", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            req.AddParameter("apiKey", "freekey");
            req.AddParameter("token", "0xad95a3c0fdc9bc4b27fd79e028a0a808d5564aa4");
            client.ExecuteAsync<EpInfo>(req, res =>
            {
                double bal = res.Data.Tokens.First(x => x.TokenInfo.Symbol == "DYT").Balance;
                fund = BigInteger.Parse(bal.ToString(), NumberStyles.Any);
                log.Info($"GetFund: OK ({fund.ToString()})");
            });
        }

        private void GetFund2(RestClient client)
        {
            var req = new RestRequest("getAddressInfo/0x26bb02501e8460c4eb461df0e9a7d598b9aef190", Method.GET);
            req.AddParameter("time", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            req.AddParameter("apiKey", "freekey");
            req.AddParameter("token", "0xad95a3c0fdc9bc4b27fd79e028a0a808d5564aa4");
            client.ExecuteAsync<EpInfo>(req, res =>
            {
                double bal = res.Data.Tokens.First(x => x.TokenInfo.Symbol == "DYT").Balance;
                fund2 = BigInteger.Parse(bal.ToString(), NumberStyles.Any);
                log.Info($"GetFund2: OK ({fund2.ToString()})");
            });
        }

        private void GetFund3(RestClient client)
        {
            var req = new RestRequest("getAddressInfo/0x010b3a9e0a199e45ebb3d08de47479ffe6a789a1", Method.GET);
            req.AddParameter("time", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            req.AddParameter("apiKey", "freekey");
            req.AddParameter("token", "0xad95a3c0fdc9bc4b27fd79e028a0a808d5564aa4");
            client.ExecuteAsync<EpInfo>(req, res =>
            {
                double bal = res.Data.Tokens.First(x => x.TokenInfo.Symbol == "DYT").Balance;
                fund3 = BigInteger.Parse(bal.ToString(), NumberStyles.Any);
                log.Info($"GetFund3: OK ({fund3.ToString()})");
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
            req.AddParameter("contractaddress", "0xad95a3c0fdc9bc4b27fd79e028a0a808d5564aa4");
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
                        To = tx.To,
                        TimeStamp = tx.TimeStamp,
                        Value = tx.Value
                    };
                    db.Add(newTx);
                }
                await db.SaveChangesAsync();
            }
        }

        private void GetBurned()
        {
            var total = BigInteger.Parse("2000000000000000000000000");
            burned = BigInteger.Subtract(total, supply);
        }

        private BigInteger GetCirculation()
        {
            var total = supply;
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