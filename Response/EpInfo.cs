using System;
using System.Collections.Generic;
using System.Text;

namespace dm.DYT.Response
{
    public class EpInfo
    {
        public string Address { get; set; }
        public EpInfoETH ETH { get; set; }
        public int CountTxs { get; set; }
        public List<EpInfoToken> Tokens { get; set; }
    }

    public class EpInfoETH
    {
        public double Balance { get; set; }
        public double TotalIn { get; set; }
        public int TotalOut { get; set; }
    }

    public class EpInfoToken
    {
        public EpInfoTokenInfo TokenInfo { get; set; }
        public double Balance { get; set; }
        public int TotalIn { get; set; }
        public int TotalOut { get; set; }
    }

    public class EpInfoTokenInfo
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public string Decimals { get; set; }
        public string Symbol { get; set; }
        public string TotalSupply { get; set; }
        public string Owner { get; set; }
        public int LastUpdated { get; set; }
        public int IssuancesCount { get; set; }
        public int HoldersCount { get; set; }
        public bool Price { get; set; }
    }
}
