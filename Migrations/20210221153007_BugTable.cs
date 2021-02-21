using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.Migrations
{
    public partial class BugTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bug",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NO = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusInVS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FixedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstimatedHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RejectedTimes = table.Column<int>(type: "int", nullable: false),
                    Developer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Team = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bug", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bug");
        }
    }
}
