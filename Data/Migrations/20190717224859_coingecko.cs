using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace dm.DYT.Data.Migrations
{
    public partial class coingecko : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BurnAvgDayMove",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "BurnLast1HMove",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "BurnLast24HMove",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "TxAvgMove",
                table: "Stats");

            migrationBuilder.AddColumn<int>(
                name: "BurnAvgDayChange",
                table: "Stats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BurnLast1HChange",
                table: "Stats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BurnLast24HChange",
                table: "Stats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Prices360",
                columns: table => new
                {
                    Price360Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    Source = table.Column<int>(nullable: false),
                    Group = table.Column<Guid>(nullable: false),
                    PriceUSD = table.Column<decimal>(type: "decimal(9, 4)", nullable: false),
                    PriceUSDChange = table.Column<int>(nullable: false),
                    PriceUSDChangePct = table.Column<decimal>(type: "decimal(10, 8)", nullable: false),
                    PriceETH = table.Column<decimal>(type: "decimal(9, 4)", nullable: false),
                    PriceETHChange = table.Column<int>(nullable: false),
                    PriceETHChangePct = table.Column<decimal>(type: "decimal(10, 8)", nullable: false),
                    PriceBTC = table.Column<decimal>(type: "decimal(16, 8)", nullable: false),
                    PriceBTCChange = table.Column<int>(nullable: false),
                    PriceBTCChangePct = table.Column<decimal>(type: "decimal(10, 8)", nullable: false),
                    MarketCapUSD = table.Column<int>(nullable: false),
                    MarketCapUSDChange = table.Column<int>(nullable: false),
                    MarketCapUSDChangePct = table.Column<decimal>(type: "decimal(10, 8)", nullable: false),
                    VolumeUSD = table.Column<int>(nullable: false),
                    VolumeUSDChange = table.Column<int>(nullable: false),
                    VolumeUSDChangePct = table.Column<decimal>(type: "decimal(10, 8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices360", x => x.Price360Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prices360_Date",
                table: "Prices360",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Prices360_Group",
                table: "Prices360",
                column: "Group");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prices360");

            migrationBuilder.DropColumn(
                name: "BurnAvgDayChange",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "BurnLast1HChange",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "BurnLast24HChange",
                table: "Stats");

            migrationBuilder.AddColumn<int>(
                name: "BurnAvgDayMove",
                table: "Stats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BurnLast1HMove",
                table: "Stats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BurnLast24HMove",
                table: "Stats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TxAvgMove",
                table: "Stats",
                nullable: false,
                defaultValue: 0);
        }
    }
}
