using Microsoft.EntityFrameworkCore.Migrations;

namespace Swift_Tomes_Accounting.Migrations
{
    public partial class fkR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Journal_Accounts_Account_AccountNumber",
                table: "Journal_Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Journal_Accounts_AccountNumber",
                table: "Journal_Accounts");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "Journal_Accounts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AccountNumber",
                table: "Journal_Accounts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_Journal_Accounts_AccountNumber",
                table: "Journal_Accounts",
                column: "AccountNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Journal_Accounts_Account_AccountNumber",
                table: "Journal_Accounts",
                column: "AccountNumber",
                principalTable: "Account",
                principalColumn: "AccountNumber",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
