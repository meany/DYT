using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using dm.DYT.Common;
using NLog;

namespace dm.DYT.DiscordBot
{
    public class Args
    {
        private readonly DiscordSocketClient discordClient;
        private static Logger log = LogManager.GetCurrentClassLogger();

        public Args(DiscordSocketClient discordClient)
        {
            this.discordClient = discordClient;
        }
    }
}