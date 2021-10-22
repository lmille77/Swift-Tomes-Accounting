using Microsoft.EntityFrameworkCore.Migrations;

namespace Swift_Tomes_Accounting.Migrations
{
    public partial class JA1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Journal_Accounts",
                columns: table => new
                {
                    JAId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountNumber = table.Column<double>(type: "float", nullable: false),
                    JournalId = table.Column<int>(type: "int", nullable: false),
                    Debit = table.Column<double>(type: "float", nullable: false),
                    Credit = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Journal_Accounts", x => x.JAId);
                    table.ForeignKey(
                        name: "FK_Journal_Accounts_Account_AccountNumber",
                        column: x => x.AccountNumber,
                        principalTable: "Account",
                        principalColumn: "AccountNumber",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Journal_Accounts_Journalizes_JournalId",
                        column: x => x.JournalId,
                        principalTable: "Journalizes",
                        principalColumn: "JournalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Journal_Accounts_AccountNumber",
                table: "Journal_Accounts",
                column: "AccountNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Journal_Accounts_JournalId",
                table: "Journal_Accounts",
                column: "JournalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Journal_Accounts");
        }
    }
}
