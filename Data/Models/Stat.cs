using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace dm.DYT.Data.Models
{
    public enum Movement
    {
        None = 0,
        Down = 1,
        Up = 2
    }

    public class Stat
    {
        public int StatId { get; set; }
        public DateTime Date { get; set; }
        public int Transactions { get; set; }
        [Column(TypeName = "decimal(9, 4)")]
        public decimal TxAvgDay { get; set; }
        public Movement TxAvgMove { get; set; }
        [Column(TypeName = "decimal(25, 18)")]
        public decimal Supply { get; set; }
        [Column(TypeName = "decimal(25, 18)")]
        public decimal Circulation { get; set; }
        [Column(TypeName = "decimal(25, 18)")]
        public decimal Burned { get; set; }
        [Column(TypeName = "decimal(25, 18)")]
        public decimal BurnLast1H { get; set; }
        public Movement BurnLast1HMove { get; set; }
        [Column(TypeName = "decimal(25, 18)")]
        public decimal BurnLast24H { get; set; }
        public Movement BurnLast24HMove { get; set; }
        [Column(TypeName = "decimal(25, 18)")]
        public decimal BurnAvgDay { get; set; }
        public Movement BurnAvgDayMove { get; set; }
    }
}
