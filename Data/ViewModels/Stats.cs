using dm.DYT.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace dm.DYT.Data.ViewModels
{
    public class Stats
    {
        public Stat Stat { get; set; }
        public WeightedPrice WeightedPrice { get; set; }
        public List<Price> Prices { get; set; }
        public bool IsOldPrice { get; set; }
    }
}
