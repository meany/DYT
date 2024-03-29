﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using dm.DYT.Data;

namespace dm.DYT.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20200530124110_from")]
    partial class from
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("dm.DYT.Data.Models.Price360", b =>
                {
                    b.Property<int>("Price360Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<Guid>("Group");

                    b.Property<int>("MarketCapUSD");

                    b.Property<int>("MarketCapUSDChange");

                    b.Property<decimal>("MarketCapUSDChangePct")
                        .HasColumnType("decimal(12, 8)");

                    b.Property<decimal>("PriceBTC")
                        .HasColumnType("decimal(16, 8)");

                    b.Property<int>("PriceBTCChange");

                    b.Property<decimal>("PriceBTCChangePct")
                        .HasColumnType("decimal(12, 8)");

                    b.Property<decimal>("PriceETH")
                        .HasColumnType("decimal(16, 8)");

                    b.Property<int>("PriceETHChange");

                    b.Property<decimal>("PriceETHChangePct")
                        .HasColumnType("decimal(12, 8)");

                    b.Property<decimal>("PriceUSD")
                        .HasColumnType("decimal(11, 6)");

                    b.Property<int>("PriceUSDChange");

                    b.Property<decimal>("PriceUSDChangePct")
                        .HasColumnType("decimal(12, 8)");

                    b.Property<int>("Source");

                    b.Property<int>("VolumeUSD");

                    b.Property<int>("VolumeUSDChange");

                    b.Property<decimal>("VolumeUSDChangePct")
                        .HasColumnType("decimal(12, 8)");

                    b.HasKey("Price360Id");

                    b.HasIndex("Date");

                    b.HasIndex("Group");

                    b.ToTable("Prices360");
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

                    b.HasIndex("Date");

                    b.HasIndex("Response", "Type");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("dm.DYT.Data.Models.Stat", b =>
                {
                    b.Property<int>("StatId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("BurnAvgDay")
                        .HasColumnType("decimal(25, 18)");

                    b.Property<int>("BurnAvgDayChange");

                    b.Property<decimal>("BurnLast1H")
                        .HasColumnType("decimal(25, 18)");

                    b.Property<int>("BurnLast1HChange");

                    b.Property<decimal>("BurnLast24H")
                        .HasColumnType("decimal(25, 18)");

                    b.Property<int>("BurnLast24HChange");

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

                    b.HasKey("StatId");

                    b.HasIndex("Date");

                    b.ToTable("Stats");
                });

            modelBuilder.Entity("dm.DYT.Data.Models.Transaction", b =>
                {
                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BlockNumber");

                    b.Property<string>("From");

                    b.Property<string>("Hash");

                    b.Property<DateTimeOffset>("TimeStamp");

                    b.Property<string>("To");

                    b.Property<string>("Value");

                    b.HasKey("TransactionId");

                    b.HasIndex("TimeStamp");

                    b.ToTable("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
