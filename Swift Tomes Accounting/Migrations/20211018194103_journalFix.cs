using Microsoft.EntityFrameworkCore.Migrations;

namespace Swift_Tomes_Accounting.Migrations
{
    public partial class journalFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Debit",
                table: "Journalizes",
                newName: "Debit2");

            migrationBuilder.RenameColumn(
                name: "Credit",
                table: "Journalizes",
                newName: "Debit1");

            migrationBuilder.RenameColumn(
                name: "Accounttwo",
                table: "Journalizes",
                newName: "Account2");

            migrationBuilder.RenameColumn(
                name: "AccountOne",
                table: "Journalizes",
                newName: "Account1");

            migrationBuilder.AddColumn<double>(
                name: "Credit1",
                table: "Journalizes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Credit2",
                table: "Journalizes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Credit1",
                table: "Journalizes");

            migrationBuilder.DropColumn(
                name: "Credit2",
                table: "Journalizes");

            migrationBuilder.RenameColumn(
                name: "Debit2",
                table: "Journalizes",
                newName: "Debit");

            migrationBuilder.RenameColumn(
                name: "Debit1",
                table: "Journalizes",
                newName: "Credit");

            migrationBuilder.RenameColumn(
                name: "Account2",
                table: "Journalizes",
                newName: "Accounttwo");

            migrationBuilder.RenameColumn(
                name: "Account1",
                table: "Journalizes",
                newName: "AccountOne");
        }
    }
}
