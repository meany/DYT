﻿using dm.DYT.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dm.DYT.Data
{
    public static class Common
    {
        public static async Task<ViewModels.Stats> GetStats(AppDbContext db)
        {
            var vm = new ViewModels.Stats();

            var stat = await db.Stats
                .AsNoTracking()
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            vm.Stat = stat;
            var prices = await db.Prices
                .AsNoTracking()
                .Where(x => x.Group == stat.Group)
                .ToListAsync()
                .ConfigureAwait(false);
            vm.Prices = prices;

            if (!prices.Any())
            {
                vm.IsOldPrice = true;
                prices = await db.Prices
                    .AsNoTracking()
                    .OrderByDescending(x => x.Date)
                    .Take(3)
                    .ToListAsync()
                    .ConfigureAwait(false);
                vm.Prices = prices;
            }

            var wprice = new ViewModels.WeightedPrice
            {
                PriceUSD = prices.Sum(x => x.PriceUSDWeighted),
                PriceBTC = prices.Sum(x => x.PriceBTCWeighted),
                PriceETH = prices.Sum(x => x.PriceETHWeighted),
                MarketCapUSD = prices.Sum(x => x.MarketCapUSDWeighted),
                VolumeUSD = prices.Sum(x => x.VolumeUSD)
            };

            vm.WeightedPrice = wprice;
            return vm;
        }
    }
}