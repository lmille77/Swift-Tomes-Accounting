using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Swift_Tomes_Accounting.Migrations
{
    public partial class errortable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "ErrorTable",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorTable", x => x.ID);
                });

           

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.DropTable(
                name: "ErrorTable");

        }
    }
}
