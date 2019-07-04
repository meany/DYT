using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace dm.DYT.Data.Migrations
{
    public partial class more1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Group",
                table: "Stats",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Base",
                table: "Prices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "Group",
                table: "Prices",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "MarketCapUSDPct",
                table: "Prices",
                type: "decimal(5, 4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceBTCPct",
                table: "Prices",
                type: "decimal(5, 4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceBTCWeighted",
                table: "Prices",
                type: "decimal(16, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceETHPct",
                table: "Prices",
                type: "decimal(5, 4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceETHWeighted",
                table: "Prices",
                type: "decimal(25, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceUSDPct",
                table: "Prices",
                type: "decimal(5, 4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceUSDWeighted",
                table: "Prices",
                type: "decimal(9, 4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "Prices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "VolumeUSDPct",
                table: "Prices",
                type: "decimal(5, 4)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Group",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Base",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "MarketCapUSDPct",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PriceBTCPct",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PriceBTCWeighted",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PriceETHPct",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PriceETHWeighted",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PriceUSDPct",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PriceUSDWeighted",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "VolumeUSDPct",
                table: "Prices");
        }
    }
}
