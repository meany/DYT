using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace dm.DYT.Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    PriceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    PriceUSD = table.Column<decimal>(type: "decimal(9, 4)", nullable: false),
                    PriceUSDMove = table.Column<int>(nullable: false),
                    MarketCapUSD = table.Column<int>(nullable: false),
                    MarketCapUSDMove = table.Column<int>(nullable: false),
                    VolumeUSD = table.Column<int>(nullable: false),
                    VolumeUSDMove = table.Column<int>(nullable: false),
                    PriceETH = table.Column<decimal>(type: "decimal(25, 18)", nullable: false),
                    PriceETHMove = table.Column<int>(nullable: false),
                    PriceBTC = table.Column<decimal>(type: "decimal(16, 8)", nullable: false),
                    PriceBTCMove = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.PriceId);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    RequestId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    User = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Response = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.RequestId);
                });

            migrationBuilder.CreateTable(
                name: "Stats",
                columns: table => new
                {
                    StatId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    Transactions = table.Column<int>(nullable: false),
                    TxAvgDay = table.Column<decimal>(type: "decimal(9, 4)", nullable: false),
                    TxAvgMove = table.Column<int>(nullable: false),
                    Supply = table.Column<decimal>(type: "decimal(25, 18)", nullable: false),
                    Circulation = table.Column<decimal>(type: "decimal(25, 18)", nullable: false),
                    Burned = table.Column<decimal>(type: "decimal(25, 18)", nullable: false),
                    BurnLast1H = table.Column<decimal>(type: "decimal(25, 18)", nullable: false),
                    BurnLast1HMove = table.Column<int>(nullable: false),
                    BurnLast24H = table.Column<decimal>(type: "decimal(25, 18)", nullable: false),
                    BurnLast24HMove = table.Column<int>(nullable: false),
                    BurnAvgDay = table.Column<decimal>(type: "decimal(25, 18)", nullable: false),
                    BurnAvgDayMove = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stats", x => x.StatId);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BlockNumber = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTimeOffset>(nullable: false),
                    Hash = table.Column<string>(nullable: true),
                    To = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "Stats");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
