using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dm.DYT.Data;
using dm.DYT.Data.Models;
using dm.DYT.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private AppDbContext db;

        public DataController(AppDbContext db)
        {
            this.db = db;
        }

        // GET data
        [HttpGet]
        public async Task<ActionResult<Stats>> Get()
        {
            return await Common.GetStats(db);
        }
    }
}
