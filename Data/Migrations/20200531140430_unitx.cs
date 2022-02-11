using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace dm.DYT.Data.Migrations
{
    public partial class unitx : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UniswapTransactions",
                columns: table => new
                {
                    UniswapTransactionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TxType = table.Column<int>(nullable: false),
                    DYT = table.Column<decimal>(type: "decimal(25, 18)", nullable: false),
                    WETH = table.Column<decimal>(type: "decimal(25, 18)", nullable: false),
                    USD = table.Column<decimal>(type: "decimal(11, 6)", nullable: false),
                    TransactionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniswapTransactions", x => x.UniswapTransactionId);
                    table.ForeignKey(
                        name: "FK_UniswapTransactions_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UniswapTransactions_TransactionId",
                table: "UniswapTransactions",
                column: "TransactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UniswapTransactions");
        }
    }
}
