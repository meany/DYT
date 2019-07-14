using Microsoft.EntityFrameworkCore.Migrations;

namespace dm.DYT.Data.Migrations
{
    public partial class indexes2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Prices_Date",
                table: "Prices",
                column: "Date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Prices_Date",
                table: "Prices");
        }
    }
}
