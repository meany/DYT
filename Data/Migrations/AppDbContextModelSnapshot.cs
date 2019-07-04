﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using dm.DYT.Data;

namespace dm.DYT.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("dm.DYT.Data.Models.Price", b =>
                {
                    b.Property<int>("PriceId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Base");

                    b.Property<DateTime>("Date");

                    b.Property<Guid>("Group");

                    b.Property<int>("MarketCapUSD");

                    b.Property<int>("MarketCapUSDMove");

                    b.Property<int>("MarketCapUSDWeighted");

                    b.Property<decimal>("PriceBTC")
                        .HasColumnType("decimal(16, 8)");

                    b.Property<int>("PriceBTCMove");

                    b.Property<decimal>("PriceBTCWeighted")
                        .HasColumnType("decimal(16, 8)");

                    b.Property<decimal>("PriceETH")
                        .HasColumnType("decimal(25, 18)");

                    b.Property<int>("PriceETHMove");

                    b.Property<decimal>("PriceETHWeighted")
                        .HasColumnType("decimal(25, 18)");

                    b.Property<decimal>("PriceUSD")
                        .HasColumnType("decimal(9, 4)");

                    b.Property<int>("PriceUSDMove");

                    b.Property<decimal>("PriceUSDWeighted")
                        .HasColumnType("decimal(9, 4)");

                    b.Property<int>("Source");

                    b.Property<int>("VolumeUSD");

                    b.Property<int>("VolumeUSDMove");

                    b.Property<decimal>("VolumeUSDPct")
                        .HasColumnType("decimal(5, 4)");

                    b.HasKey("PriceId");

                    b.ToTable("Prices");
                });

            modelBuilder.Entity("dm.DYT.Data.Models.Request", b =>
                {
                    b.Property<int>("RequestId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<int>("Response");

                    b.Property<int>("Type");

                    b.Property<string>("User");

                    b.HasKey("RequestId");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("dm.DYT.Data.Models.Stat", b =>
                {
                    b.Property<int>("StatId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("BurnAvgDay")
                        .HasColumnType("decimal(25, 18)");

                    b.Property<int>("BurnAvgDayMove");

                    b.Property<decimal>("BurnLast1H")
                        .HasColumnType("decimal(25, 18)");

                    b.Property<int>("BurnLast1HMove");

                    b.Property<decimal>("BurnLast24H")
                        .HasColumnType("decimal(25, 18)");

                    b.Property<int>("BurnLast24HMove");

                    b.Property<decimal>("Burned")
                        .HasColumnType("decimal(25, 18)");

                    b.Property<decimal>("Circulation")
                        .HasColumnType("decimal(25, 18)");

                    b.Property<DateTime>("Date");

                    b.Property<Guid>("Group");

                    b.Property<decimal>("Supply")
                        .HasColumnType("decimal(25, 18)");

                    b.Property<int>("Transactions");

                    b.Property<decimal>("TxAvgDay")
                        .HasColumnType("decimal(9, 4)");

                    b.Property<int>("TxAvgMove");

                    b.HasKey("StatId");

                    b.ToTable("Stats");
                });

            modelBuilder.Entity("dm.DYT.Data.Models.Transaction", b =>
                {
                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BlockNumber");

                    b.Property<string>("Hash");

                    b.Property<DateTimeOffset>("TimeStamp");

                    b.Property<string>("To");

                    b.Property<string>("Value");

                    b.HasKey("TransactionId");

                    b.ToTable("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
