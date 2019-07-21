using Microsoft.EntityFrameworkCore.Migrations;

namespace dm.DYT.Data.Migrations
{
    public partial class moredecimals2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "VolumeUSDChangePct",
                table: "Prices360",
                type: "decimal(12, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceUSDChangePct",
                table: "Prices360",
                type: "decimal(12, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceETHChangePct",
                table: "Prices360",
                type: "decimal(12, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceBTCChangePct",
                table: "Prices360",
                type: "decimal(12, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MarketCapUSDChangePct",
                table: "Prices360",
                type: "decimal(12, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 8)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "VolumeUSDChangePct",
                table: "Prices360",
                type: "decimal(10, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceUSDChangePct",
                table: "Prices360",
                type: "decimal(10, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceETHChangePct",
                table: "Prices360",
                type: "decimal(10, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceBTCChangePct",
                table: "Prices360",
                type: "decimal(10, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MarketCapUSDChangePct",
                table: "Prices360",
                type: "decimal(10, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12, 8)");
        }
    }
}
