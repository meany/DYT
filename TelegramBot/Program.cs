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
using Telegram.Bot.Args;

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

                await RunBot();
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

                log.Info($"Bot connected");

                if (config.ChatId == 0)
                {
                    log.Info("ChatId = 0, waiting for messages");
                    botClient.OnMessage += BotClient_OnMessage;
                    botClient.StartReceiving();
                    await Task.Delay(-1).ConfigureAwait(false);
                }
                else
                {
                    var item = await Data.Common.GetStats(db);

                    string text = $"🔥 {item.Stat.BurnLast1H.FormatDyt()} $DYT burned in the last hour\n" +
                        $"🔥 {item.Stat.BurnLast24H.FormatDyt()} $DYT burned in the last 24 hours\n\n" +
                        $"🤝 Transactions: {item.Stat.Transactions.Format()}\n" +
                        $"📃 Supply: {item.Stat.Supply.FormatDyt()} $DYT\n" +
                        $"🔥 Burned: {item.Stat.Burned.FormatDyt()} (Rate: {item.Stat.BurnAvgDay.FormatDyt()}/day)\n" +
                        $"🤑 Price/USD: ${item.Price.PriceUSD.FormatUsd()}\n" +
                        $"🤑 Price/BTC: ₿{item.Price.PriceBTC.FormatBtc()}\n" +
                        $"🤑 Price/ETH: Ξ{item.Price.PriceETH.FormatEth()}\n" +
                        $"📈 Market Cap: ${item.Price.MarketCapUSD.FormatLarge()}\n" +
                        $"💸 Volume: ${item.Price.VolumeUSD.FormatLarge()}";

                    await botClient.SendTextMessageAsync(
                      chatId: config.ChatId,
                      text: text
                    );

                    log.Info("Stats sent");

                    if (item.IsOutOfSync())
                    {
                        await botClient.SendTextMessageAsync(
                          chatId: config.ChatId,
                          text: "Stats might be out of sync. The admin has been contacted."
                        );

                        await botClient.SendTextMessageAsync(
                          chatId: config.AdminId,
                          text: "Stats out of sync."
                        );

                        log.Info("Price out of sync.");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void BotClient_OnMessage(object sender, MessageEventArgs e)
        {
            log.Info($"ChatId: {e.Message.Chat.Id}, Message: {e.Message.Text}");
        }
    }
}
