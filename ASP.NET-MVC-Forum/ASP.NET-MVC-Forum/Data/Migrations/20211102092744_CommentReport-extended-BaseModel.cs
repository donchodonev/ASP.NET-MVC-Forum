using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASP.NET_MVC_Forum.Data.Migrations
{
    public partial class CommentReportextendedBaseModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "CommentReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "CommentReports",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CommentReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "CommentReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_CommentId",
                table: "CommentReports",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReports_Comments_CommentId",
                table: "CommentReports",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentReports_Comments_CommentId",
                table: "CommentReports");

            migrationBuilder.DropIndex(
                name: "IX_CommentReports_CommentId",
                table: "CommentReports");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "CommentReports");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "CommentReports");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CommentReports");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "CommentReports");
        }
    }
}
