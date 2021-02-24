using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.Migrations
{
    public partial class ResovedReason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResolvedReason",
                table: "Bug",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResolvedReason",
                table: "Bug");
        }
    }
}
