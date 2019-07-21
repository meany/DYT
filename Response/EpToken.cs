using System;
using System.Collections.Generic;
using System.Text;

namespace dm.DYT.Response
{
    public class EpToken
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public string Decimals { get; set; }
        public string Symbol { get; set; }
        public string TotalSupply { get; set; }
        public string Owner { get; set; }
        public int TransfersCount { get; set; }
        public int LastUpdated { get; set; }
        public int IssuancesCount { get; set; }
        public int HoldersCount { get; set; }
        public int EthTransfersCount { get; set; }
        public bool Price { get; set; }
        public int CountOps { get; set; }
    }
}
