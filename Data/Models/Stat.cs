using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace dm.DYT.Data.Models
{
    public enum Change
    {
        None = 0,
        Down = 1,
        Up = 2
    }

    public class Stat
    {
        public int StatId { get; set; }
        public DateTime Date { get; set; }
        public Guid Group { get; set; }
        public int Transactions { get; set; }
        [Column(TypeName = "decimal(9, 4)")]
        public decimal TxAvgDay { get; set; }
        [Column(TypeName = "decimal(25, 18)")]
        public decimal Supply { get; set; }
        [Column(TypeName = "decimal(25, 18)")]
        public decimal Circulation { get; set; }
        [Column(TypeName = "decimal(25, 18)")]
        public decimal Burned { get; set; }
        [Column(TypeName = "decimal(25, 18)")]
        public decimal BurnLast1H { get; set; }
        public Change BurnLast1HChange { get; set; }
        [Column(TypeName = "decimal(25, 18)")]
        public decimal BurnLast24H { get; set; }
        public Change BurnLast24HChange { get; set; }
        [Column(TypeName = "decimal(25, 18)")]
        public decimal BurnAvgDay { get; set; }
        public Change BurnAvgDayChange { get; set; }
    }
}
