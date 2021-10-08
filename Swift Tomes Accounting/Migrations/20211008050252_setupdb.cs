using Microsoft.EntityFrameworkCore.Migrations;

namespace Swift_Tomes_Accounting.Migrations
{
    public partial class setupdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountDB",
                table: "AccountDB");

            migrationBuilder.RenameTable(
                name: "AccountDB",
                newName: "Account");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Account",
                table: "Account",
                columns: new[] { "AccountNumber", "AccountName" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Account",
                table: "Account");

            migrationBuilder.RenameTable(
                name: "Account",
                newName: "AccountDB");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountDB",
                table: "AccountDB",
                columns: new[] { "AccountNumber", "AccountName" });
        }
    }
}
