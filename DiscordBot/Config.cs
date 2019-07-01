using System.Collections.Generic;

namespace dm.DYT.DiscordBot
{
    public class Config
    {
        public bool BotHelp { get; set; }
        public string BotName { get; set; }
        public string BotPlaying { get; set; }
        public string BotPrefix { get; set; }
        public string BotToken { get; set; }
        public List<ulong> ChannelIds { get; set; }
        public string EmoteGood { get; set; }
        public string EmoteBad { get; set; }
        public string EmoteDynamite { get; set; }
        public int RequestCooldown { get; set; }
    }
}
