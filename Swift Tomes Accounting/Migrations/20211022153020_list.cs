using Microsoft.EntityFrameworkCore.Migrations;

namespace Swift_Tomes_Accounting.Migrations
{
    public partial class list : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JournalizeJournalId",
                table: "Account",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_JournalizeJournalId",
                table: "Account",
                column: "JournalizeJournalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Journalizes_JournalizeJournalId",
                table: "Account",
                column: "JournalizeJournalId",
                principalTable: "Journalizes",
                principalColumn: "JournalId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Journalizes_JournalizeJournalId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_JournalizeJournalId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "JournalizeJournalId",
                table: "Account");
        }
    }
}
