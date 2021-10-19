using Microsoft.EntityFrameworkCore.Migrations;

namespace Swift_Tomes_Accounting.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Journalize_Account_AccountNumber",
                table: "Journalize");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Journalize",
                table: "Journalize");

            migrationBuilder.RenameTable(
                name: "Journalize",
                newName: "Journalizes");

            migrationBuilder.RenameIndex(
                name: "IX_Journalize_AccountNumber",
                table: "Journalizes",
                newName: "IX_Journalizes_AccountNumber");

            migrationBuilder.AddColumn<double>(
                name: "Balance",
                table: "Journalizes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Credit",
                table: "Journalizes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Debit",
                table: "Journalizes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "MyProperty",
                table: "Journalizes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Journalizes",
                table: "Journalizes",
                column: "JournalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Journalizes_Account_AccountNumber",
                table: "Journalizes",
                column: "AccountNumber",
                principalTable: "Account",
                principalColumn: "AccountNumber",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Journalizes_Account_AccountNumber",
                table: "Journalizes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Journalizes",
                table: "Journalizes");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Journalizes");

            migrationBuilder.DropColumn(
                name: "Credit",
                table: "Journalizes");

            migrationBuilder.DropColumn(
                name: "Debit",
                table: "Journalizes");

            migrationBuilder.DropColumn(
                name: "MyProperty",
                table: "Journalizes");

            migrationBuilder.RenameTable(
                name: "Journalizes",
                newName: "Journalize");

            migrationBuilder.RenameIndex(
                name: "IX_Journalizes_AccountNumber",
                table: "Journalize",
                newName: "IX_Journalize_AccountNumber");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Journalize",
                table: "Journalize",
                column: "JournalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Journalize_Account_AccountNumber",
                table: "Journalize",
                column: "AccountNumber",
                principalTable: "Account",
                principalColumn: "AccountNumber",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
