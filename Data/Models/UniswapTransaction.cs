using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace dm.DYT.Data.Models
{
    public enum UniswapTransactionType
    {
        Buy = 0,
        Sell = 1
    }

    public class UniswapTransaction
    {
        public int UniswapTransactionId { get; set; }
        public UniswapTransactionType TxType { get; set; }
        [Column(TypeName = "decimal(25, 18)")]
        public decimal DYT { get; set; }
        [Column(TypeName = "decimal(25, 18)")]
        public decimal WETH { get; set; }
        [Column(TypeName = "decimal(11, 6)")]
        public decimal USD { get; set; }

        public Transaction Transaction { get; set; }
    }
}
