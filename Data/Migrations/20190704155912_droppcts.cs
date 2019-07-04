using Microsoft.EntityFrameworkCore.Migrations;

namespace dm.DYT.Data.Migrations
{
    public partial class droppcts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarketCapUSDPct",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PriceBTCPct",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PriceETHPct",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PriceUSDPct",
                table: "Prices");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "PriceETHPct",
                table: "Prices",
                type: "decimal(5, 4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceUSDPct",
                table: "Prices",
                type: "decimal(5, 4)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
