using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.Migrations
{
    public partial class FirstHumanCommentDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FirstPullRequestCommentDate",
                table: "Bug",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FirstPullRequestIterationCount",
                table: "Bug",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstPullRequestCommentDate",
                table: "Bug");

            migrationBuilder.DropColumn(
                name: "FirstPullRequestIterationCount",
                table: "Bug");
        }
    }
}
