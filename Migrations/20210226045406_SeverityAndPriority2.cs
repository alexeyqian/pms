using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.Migrations
{
    public partial class SeverityAndPriority2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Bug",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "Bug",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Bug");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "Bug");
        }
    }
}
