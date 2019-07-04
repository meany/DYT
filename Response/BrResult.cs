using System;
using System.Collections.Generic;
using System.Text;

namespace dm.DYT.Response
{
    public class BrStats
    {
        public int NumBidsWithinRange { get; set; }
        public int NumAsksWithinRange { get; set; }
        public string BaseTokenAvailable { get; set; }
        public string QuoteTokenAvailable { get; set; }
        public string Volume24Hour { get; set; }
        public string PercentChange24Hour { get; set; }
    }

    public class BrTicker
    {
        public string TransactionHash { get; set; }
        public string Price { get; set; }
        public string Size { get; set; }
        public long Timestamp { get; set; }
        public string BestBid { get; set; }
        public string BestAsk { get; set; }
        public string SpreadPercentage { get; set; }
    }

    public class BrResult
    {
        public string Id { get; set; }
        public BrStats Stats { get; set; }
        public BrTicker Ticker { get; set; }
    }
}
