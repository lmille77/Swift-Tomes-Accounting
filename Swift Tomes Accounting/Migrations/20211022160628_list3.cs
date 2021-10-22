using Microsoft.EntityFrameworkCore.Migrations;

namespace Swift_Tomes_Accounting.Migrations
{
    public partial class list3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Journalizes_Journal_Accounts_Journal_AccountsJAId",
                table: "Journalizes");

            migrationBuilder.DropIndex(
                name: "IX_Journalizes_Journal_AccountsJAId",
                table: "Journalizes");

            migrationBuilder.DropColumn(
                name: "Journal_AccountsJAId",
                table: "Journalizes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Journal_AccountsJAId",
                table: "Journalizes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Journalizes_Journal_AccountsJAId",
                table: "Journalizes",
                column: "Journal_AccountsJAId");

            migrationBuilder.AddForeignKey(
                name: "FK_Journalizes_Journal_Accounts_Journal_AccountsJAId",
                table: "Journalizes",
                column: "Journal_AccountsJAId",
                principalTable: "Journal_Accounts",
                principalColumn: "JAId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
