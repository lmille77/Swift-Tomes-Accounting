using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Swift_Tomes_Accounting.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.CreateTable(
                name: "EventJournal",
                columns: table => new
                {
                    eventID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    journalId = table.Column<int>(type: "int", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isApproved = table.Column<bool>(type: "bit", nullable: false),
                    isDenied = table.Column<bool>(type: "bit", nullable: false),
                    eventTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    eventType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    eventPerformedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventJournal", x => x.eventID);
                });

           

            migrationBuilder.CreateTable(
                name: "Journalizes",
                columns: table => new
                {
                    JournalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    isApproved = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    docUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRejected = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isCJE = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Journalizes", x => x.JournalId);
                });

           

            migrationBuilder.CreateTable(
                name: "Journal_Accounts",
                columns: table => new
                {
                    JAId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalId = table.Column<int>(type: "int", nullable: false),
                    Debit = table.Column<double>(type: "float", nullable: false),
                    Credit = table.Column<double>(type: "float", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountName1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountName2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRejected = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Journal_Accounts", x => x.JAId);
                    table.ForeignKey(
                        name: "FK_Journal_Accounts_Journalizes_JournalId",
                        column: x => x.JournalId,
                        principalTable: "Journalizes",
                        principalColumn: "JournalId",
                        onDelete: ReferentialAction.Cascade);
                });

            
         

           

            migrationBuilder.CreateIndex(
                name: "IX_Journal_Accounts_JournalId",
                table: "Journal_Accounts",
                column: "JournalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropTable(
                name: "EventJournal");

           

            migrationBuilder.DropTable(
                name: "Journal_Accounts");

          

            migrationBuilder.DropTable(
                name: "Journalizes");
        }
    }
}
