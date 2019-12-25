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
            var syncMin = (Stat.Date - Price.Date).TotalMinutes;
            if (syncMin > 30 || syncMin < -30)
                return true;

            return false;
        }
    }
}
