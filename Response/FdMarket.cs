using System;
using System.Collections.Generic;
using System.Text;

namespace dm.DYT.Response
{
    public class FdMarket
    {
        public List<Trade> Trades { get; set; }
    }

    public class Trade
    {
        public string TxHash { get; set; }
        public DateTime Date { get; set; }
        public string Price { get; set; }
        public string Side { get; set; }
        public string Amount { get; set; }
        public string AmountBase { get; set; }
        public string Buyer { get; set; }
        public string Seller { get; set; }
        public string TokenAddr { get; set; }
    }
}