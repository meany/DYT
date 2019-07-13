using Microsoft.EntityFrameworkCore.Migrations;

namespace dm.DYT.Data.Migrations
{
    public partial class indexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TimeStamp",
                table: "Transactions",
                column: "TimeStamp");

            migrationBuilder.CreateIndex(
                name: "IX_Stats_Date",
                table: "Stats",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Date",
                table: "Requests",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Response_Type",
                table: "Requests",
                columns: new[] { "Response", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_Prices_Group",
                table: "Prices",
                column: "Group");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_TimeStamp",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Stats_Date",
                table: "Stats");

            migrationBuilder.DropIndex(
                name: "IX_Requests_Date",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_Response_Type",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Prices_Group",
                table: "Prices");
        }
    }
}
