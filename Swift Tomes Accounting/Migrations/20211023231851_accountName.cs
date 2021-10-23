using Microsoft.EntityFrameworkCore.Migrations;

namespace Swift_Tomes_Accounting.Migrations
{
    public partial class accountName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccountName",
                table: "Journal_Accounts",
                newName: "AccountName2");

            migrationBuilder.AddColumn<string>(
                name: "AccountName1",
                table: "Journal_Accounts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountName1",
                table: "Journal_Accounts");

            migrationBuilder.RenameColumn(
                name: "AccountName2",
                table: "Journal_Accounts",
                newName: "AccountName");
        }
    }
}
