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

                    var newReq = new Request
                    {
                        Type = RequestType.Price,
                        User = Context.User.ToString(),
                    };

                    var req = await db.Requests
                        .AsNoTracking()
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

                            return;
                        }
                    }

                    var emotes = new Emotes(Context);
                    var dynamite = await emotes.Get(config.EmoteDynamite).ConfigureAwait(false);

                    var stat = await db.Stats
                        .AsNoTracking()
                        .OrderByDescending(x => x.Date)
                        .FirstAsync()
                        .ConfigureAwait(false);
                    var price = await db.Prices
                        .AsNoTracking()
                        .OrderByDescending(x => x.Date)
                        .FirstAsync()
                        .ConfigureAwait(false);

                    var title = $"Current Price and Statistics";
                    var output = new EmbedBuilder();
                    output.WithColor(Color.DYNAMITE_RED)
                    .WithAuthor(author =>
                    {
                        author.WithName(title);
                    })
                    .WithDescription($"**{stat.BurnLast24H.FormatDyt()} DYT** have been burned in the last 24 hours! {dynamite}")
                    .AddField($"— Market (ForkDelta/{price.Date.ToDate()})", "```ml\n" +
                        $"Price/USD:   ${price.PriceUSD.FormatUsd()}\n" +
                        $"Price/BTC:   ₿{price.PriceBTC.FormatBtc()}\n" +
                        $"Price/ETH:   Ξ{price.PriceETH.FormatDyt()}\n" +
                        $"Market Cap:  ${price.MarketCapUSD.FormatLarge()}\n" +
                        $"Volume/24H:  ${price.VolumeUSD.FormatLarge()}" +
                        "```")
                    .AddField($"— Statistics (DYT/{stat.Date.ToDate()})", "```ml\n" +
                        $"Transactions:   {stat.Transactions.Format()}\n" +
                        $"Total Supply:   {stat.Supply.FormatDyt()}\n" +
                        $"Circulation:    {stat.Circulation.FormatDyt()}\n" +
                        $"Total Burned:   {stat.Burned.FormatDyt()}\n" +
                        $"Burn/Last/1H:   {stat.BurnLast1H.FormatDyt()}\n" +
                        $"Burn/Last/24H:  {stat.BurnLast24H.FormatDyt()}\n" +
                        $"Burn/Avg/Day:   {stat.BurnAvgDay.FormatDyt()}\n" +
                        "```")
                    .WithFooter(footer =>
                    {
                        footer.WithText($"Powered by Etherscan.io APIs");
                    });

                    await Discord.ReplyAsync(Context, output, deleteUserMessage: false).ConfigureAwait(false);

                    newReq.Response = RequestResponse.OK;
                    newReq.Date = DateTime.UtcNow;
                    db.Add(newReq);
                    await db.SaveChangesAsync().ConfigureAwait(false);
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