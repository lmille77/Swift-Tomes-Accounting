using Microsoft.EntityFrameworkCore.Migrations;

namespace Swift_Tomes_Accounting.Migrations
{
    public partial class accounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountOne",
                table: "Journalizes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Accounttwo",
                table: "Journalizes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountOne",
                table: "Journalizes");

            migrationBuilder.DropColumn(
                name: "Accounttwo",
                table: "Journalizes");
        }
    }
}
