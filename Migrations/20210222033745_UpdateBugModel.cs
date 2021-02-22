using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.Migrations
{
    public partial class UpdateBugModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentCount",
                table: "Bug",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Bug",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "FirstPullRequestCommentCount",
                table: "Bug",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FirstPullRequestCommitCount",
                table: "Bug",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstPullRequestDate",
                table: "Bug",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstPullRequestStatus",
                table: "Bug",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PullRequestCount",
                table: "Bug",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Bug",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Bug",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentCount",
                table: "Bug");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Bug");

            migrationBuilder.DropColumn(
                name: "FirstPullRequestCommentCount",
                table: "Bug");

            migrationBuilder.DropColumn(
                name: "FirstPullRequestCommitCount",
                table: "Bug");

            migrationBuilder.DropColumn(
                name: "FirstPullRequestDate",
                table: "Bug");

            migrationBuilder.DropColumn(
                name: "FirstPullRequestStatus",
                table: "Bug");

            migrationBuilder.DropColumn(
                name: "PullRequestCount",
                table: "Bug");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Bug");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Bug");
        }
    }
}
