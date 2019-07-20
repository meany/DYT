using Microsoft.EntityFrameworkCore.Migrations;

namespace dm.DYT.Data.Migrations
{
    public partial class moredecimals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PriceUSD",
                table: "Prices360",
                type: "decimal(11, 6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9, 4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceETH",
                table: "Prices360",
                type: "decimal(16, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9, 4)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PriceUSD",
                table: "Prices360",
                type: "decimal(9, 4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(11, 6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceETH",
                table: "Prices360",
                type: "decimal(9, 4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(16, 8)");
        }
    }
}
