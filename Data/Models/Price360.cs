using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace dm.DYT.Data.Models
{
    public enum Price360Source
    {
        CoinGecko = 0
    }

    public class Price360
    {
        public int Price360Id { get; set; }
        public DateTime Date { get; set; }
        public Price360Source Source { get; set; }
        public Guid Group { get; set; }

        [Column(TypeName = "decimal(9, 4)")]
        public decimal PriceUSD { get; set; }
        public Change PriceUSDChange { get; set; }
        [Column(TypeName = "decimal(10, 8)")]
        public decimal PriceUSDChangePct { get; set; }
        [Column(TypeName = "decimal(9, 4)")]
        public decimal PriceETH { get; set; }
        public Change PriceETHChange { get; set; }
        [Column(TypeName = "decimal(10, 8)")]
        public decimal PriceETHChangePct { get; set; }
        [Column(TypeName = "decimal(16, 8)")]
        public decimal PriceBTC { get; set; }
        public Change PriceBTCChange { get; set; }
        [Column(TypeName = "decimal(10, 8)")]
        public decimal PriceBTCChangePct { get; set; }

        public int MarketCapUSD { get; set; }
        public Change MarketCapUSDChange { get; set; }
        [Column(TypeName = "decimal(10, 8)")]
        public decimal MarketCapUSDChangePct { get; set; }

        public int VolumeUSD { get; set; }
        public Change VolumeUSDChange { get; set; }
        [Column(TypeName = "decimal(10, 8)")]
        public decimal VolumeUSDChangePct { get; set; }
    }
}
