using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace dm.DYT.Data.Models
{
    public class Price
    {
        public int PriceId { get; set; }
        public DateTime Date { get; set; }
        [Column(TypeName = "decimal(9, 4)")]
        public decimal PriceUSD { get; set; }
        public Movement PriceUSDMove { get; set; }
        public int MarketCapUSD { get; set; }
        public Movement MarketCapUSDMove { get; set; }
        public int VolumeUSD { get; set; }
        public Movement VolumeUSDMove { get; set; }

        [Column(TypeName = "decimal(25, 18)")]
        public decimal PriceETH { get; set; }
        public Movement PriceETHMove { get; set; }
        [Column(TypeName = "decimal(16, 8)")]
        public decimal PriceBTC { get; set; }
        public Movement PriceBTCMove { get; set; }
    }
}
