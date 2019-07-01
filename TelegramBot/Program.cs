using dm.DYT.Common;
using dm.DYT.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace dm.DYT.TelegramBot
{
    public class Program
    {
        private ITelegramBotClient botClient;
        private IServiceProvider services;
        private IConfigurationRoot configuration;
        private Config config;
        private AppDbContext db;
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
            => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Config.TelegramBot.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("Config.TelegramBot.Local.json", optional: true, reloadOnChange: true);

                configuration = builder.Build();

                services = new ServiceCollection()
                    .Configure<Config>(configuration)
                    .AddDatabase<AppDbContext>(configuration.GetConnectionString("Database"))
                    .BuildServiceProvider();
                config = services.GetService<IOptions<Config>>().Value;
                db = services.GetService<AppDbContext>();
                db.Database.Migrate();

                await RunBot().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async Task RunBot()
        {
            try
            {
                botClient = new TelegramBotClient(config.BotToken);
                log.Info("Bot connected.");

                var stat = await db.Stats
                    .AsNoTracking()
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                var price = await db.Prices
                    .AsNoTracking()
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                string text = $"🔥 {stat.BurnLast1H.FormatDyt()} $DYT burned in the last hour\n" +
                    $"🔥 {stat.BurnLast24H.FormatDyt()} $DYT burned in the last 24 hours\n\n" +
                    $"🤝 Transactions: {stat.Transactions.Format()}\n" +
                    $"📃 Supply: {stat.Supply.FormatDyt()} $DYT\n" +
                    $"🔁 Circulation: {stat.Circulation.FormatDyt()} $DYT\n" +
                    $"🔥 Burned: {stat.Burned.FormatDyt()} (Rate: {stat.BurnAvgDay.FormatDyt()}/day)\n" +
                    $"🤑 Price/USD: ${price.PriceUSD.FormatUsd()}\n" +
                    $"🤑 Price/BTC: ₿{price.PriceBTC.FormatBtc()}\n" +
                    $"🤑 Price/ETH: Ξ{price.PriceETH.FormatEth()}\n" +
                    $"📈 Market Cap: ${price.MarketCapUSD.FormatLarge()}\n" +
                    $"💸 Volume: ${price.VolumeUSD.FormatLarge()}";

                await botClient.SendTextMessageAsync(
                  chatId: config.ChatId,
                  text: text
                );

                log.Info("Stats sent.");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
