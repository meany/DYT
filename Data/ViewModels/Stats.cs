using dm.DYT.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace dm.DYT.Data.ViewModels
{
    public class Stats
    {
        public Stat Stat { get; set; }
        public Price360 Price { get; set; }

        public bool IsOutOfSync()
        {
            var oosStat = Stat.Date.AddMinutes(30) <= DateTime.UtcNow;
            var oosPrice = Price.Date.AddMinutes(30) <= DateTime.UtcNow;
            return (oosStat || oosPrice);
        }
    }
}
