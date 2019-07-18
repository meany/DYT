using Discord;
using Discord.Commands;
using dm.DYT.Common;
using dm.DYT.Data;
using dm.DYT.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace dm.DYT.DiscordBot.Modules
{
    public class PriceModule : ModuleBase
    {
        private readonly Config config;
        private readonly AppDbContext db;
        private static Logger log = LogManager.GetCurrentClassLogger();

        public PriceModule(IOptions<Config> config, AppDbContext db)
        {
            this.config = config.Value;
            this.db = db;
        }

        [Command("price")]
        [Summary("Displays the price and burn stats of Dynamite (DYT).")]
        [Alias("p", "stats")]
        public async Task Price()
        {
            try
            {
                if (Context.Channel is IDMChannel)
                {
                    await Discord.ReplyAsync(Context,
                        message: "Please make this request in one of the official channels.");

                    return;
                }

                if (!config.ChannelIds.Contains(Context.Channel.Id))
                    return;

                using (var a = Context.Channel.EnterTypingState())
                {
                    log.Info("Requesting prices and stats");

                    var newReq = new Request
                    {
                        Type = RequestType.Price,
                        User = Context.User.ToString(),
                    };

                    var req = await db.Requests
                        .Where(x => x.Type == RequestType.Price
                            && x.Response == RequestResponse.OK)
                        .OrderByDescending(x => x.Date)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);

                    if (req != null)
                    {
                        var secs = (req.Date.AddSeconds(config.RequestCooldown) - DateTime.UtcNow);
                        if (secs.TotalSeconds > 0)
                        {
                            await Discord.ReplyAsync(Context,
                                message: $"Requesting too fast. Please wait {Math.Ceiling(secs.TotalSeconds)} more seconds.");

                            newReq.Response = RequestResponse.RateLimited;
                            newReq.Date = DateTime.UtcNow;
                            db.Add(newReq);
                            await db.SaveChangesAsync().ConfigureAwait(false);
                            log.Info("Request rate limited");

                            return;
                        }
                    }

                    var emotes = new Emotes(Context);
                    var dynamite = await emotes.Get(config.EmoteDynamite).ConfigureAwait(false);

                    var item = await Data.Common.GetStats(db).ConfigureAwait(false);

                    log.Debug($"Prices for stat group {item.Stat.Group} not found");

                    var title = $"Current Price and Statistics";
                    var output = new EmbedBuilder();
                    output.WithColor(Color.DYNAMITE_RED)
                    .WithAuthor(author =>
                    {
                        author.WithName(title);
                    })
                    .WithDescription($"**{item.Stat.BurnLast24H.FormatDyt()} DYT** have been burned in the last 24 hours! {dynamite}")
                    .AddField($"— Market (Weighted Average)", "```ml\n" +
                        $"Price/USD:   ${item.Price.PriceUSD.FormatUsd()}\n" +
                        $"Price/BTC:   ₿{item.Price.PriceBTC.FormatBtc()}\n" +
                        $"Price/ETH:   Ξ{item.Price.PriceETH.FormatDyt(false)}\n" +
                        $"Market Cap:  ${item.Price.MarketCapUSD.FormatLarge()}\n" +
                        $"Volume/24H:  ${item.Price.VolumeUSD.FormatLarge()}" +
                        "```")
                    .AddField($"— Statistics (DYT)", "```ml\n" +
                        $"Transactions:   {item.Stat.Transactions.Format()}\n" +
                        $"Total Supply:   {item.Stat.Supply.FormatDyt()}\n" +
                        $"Circulation:    {item.Stat.Circulation.FormatDyt()}\n" +
                        $"Total Burned:   {item.Stat.Burned.FormatDyt()} (Rate: {item.Stat.BurnAvgDay.FormatDyt()}/day)\n" +
                        $"Burn/Last/1H:   {item.Stat.BurnLast1H.FormatDyt()}\n" +
                        $"Burn/Last/24H:  {item.Stat.BurnLast24H.FormatDyt()}" +
                        "```")
                    .WithFooter(footer =>
                    {
                        footer.WithText($"{item.Price.Date.ToDate()}. Powered by Etherscan.io & CoinGecko APIs.");
                    });

                    await Discord.ReplyAsync(Context, output, deleteUserMessage: false).ConfigureAwait(false);

                    newReq.Response = RequestResponse.OK;
                    newReq.Date = DateTime.UtcNow;
                    db.Add(newReq);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                    log.Info("Prices and stats successfully sent");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }
    }
}