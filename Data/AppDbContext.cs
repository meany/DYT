using dm.DYT.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace dm.DYT.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Price> Prices { get; set; }
        public DbSet<Stat> Stats { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }

    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Config.Data.json", optional: true, reloadOnChange: true)
                .AddJsonFile("Config.Data.Local.json", optional: true, reloadOnChange: true)
                .Build();

            builder.UseSqlServer(configuration.GetConnectionString("Database"));
            return new AppDbContext(builder.Options);
        }
    }
}