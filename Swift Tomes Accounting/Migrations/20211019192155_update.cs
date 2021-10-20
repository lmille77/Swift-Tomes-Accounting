using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Swift_Tomes_Accounting.Migrations
{
    public partial class update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.CreateTable(
                name: "Journalizes",
                columns: table => new
                {
                    JournalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Account2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MyProperty = table.Column<int>(type: "int", nullable: false),
                    Debit1 = table.Column<double>(type: "float", nullable: false),
                    Credit1 = table.Column<double>(type: "float", nullable: false),
                    Debit2 = table.Column<double>(type: "float", nullable: false),
                    Credit2 = table.Column<double>(type: "float", nullable: false),
                    Balance = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isApproved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Journalizes", x => x.JournalId);
                });
        }

           

        protected override void Down(MigrationBuilder migrationBuilder)
        {            

            migrationBuilder.DropTable(
                name: "Journalizes");

            
        }
    }
}
