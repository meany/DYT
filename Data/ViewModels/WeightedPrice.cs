using System;
using System.Collections.Generic;
using System.Text;

namespace dm.DYT.Data.ViewModels
{
    public class WeightedPrice
    {
        public decimal PriceUSD { get; set; }
        public decimal PriceETH { get; set; }
        public decimal PriceBTC { get; set; }
        public int MarketCapUSD { get; set; }
        public int VolumeUSD { get; set; }
    }
}
