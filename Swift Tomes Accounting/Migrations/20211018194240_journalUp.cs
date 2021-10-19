using Microsoft.EntityFrameworkCore.Migrations;

namespace Swift_Tomes_Accounting.Migrations
{
    public partial class journalUp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Journalizes_Account_AccountNumber",
                table: "Journalizes");

            migrationBuilder.DropIndex(
                name: "IX_Journalizes_AccountNumber",
                table: "Journalizes");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "Journalizes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AccountNumber",
                table: "Journalizes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_Journalizes_AccountNumber",
                table: "Journalizes",
                column: "AccountNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Journalizes_Account_AccountNumber",
                table: "Journalizes",
                column: "AccountNumber",
                principalTable: "Account",
                principalColumn: "AccountNumber",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
