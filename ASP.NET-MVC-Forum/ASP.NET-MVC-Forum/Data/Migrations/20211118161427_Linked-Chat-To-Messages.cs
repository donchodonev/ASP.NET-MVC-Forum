using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASP.NET_MVC_Forum.Data.Migrations
{
    public partial class LinkedChatToMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_ChatUserA_ChatUserB",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatUserA_ChatUserB",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chats",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "ChatUserA",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ChatUserB",
                table: "Messages");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Messages",
                type: "nvarchar(max)",
                maxLength: 10000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 10000,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChatId",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "ChatId1",
                table: "Messages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserB",
                table: "Chats",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "UserA",
                table: "Chats",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Chats",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Chats",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Chats",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chats",
                table: "Chats",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId1",
                table: "Messages",
                column: "ChatId1");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_UserId",
                table: "Chats",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_BaseUsers_UserId",
                table: "Chats",
                column: "UserId",
                principalTable: "BaseUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_ChatId1",
                table: "Messages",
                column: "ChatId1",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_BaseUsers_UserId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_ChatId1",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatId1",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chats",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_UserId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ChatId1",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Chats");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Messages",
                type: "nvarchar(max)",
                maxLength: 10000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 10000);

            migrationBuilder.AddColumn<string>(
                name: "ChatUserA",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChatUserB",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserB",
                table: "Chats",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "UserA",
                table: "Chats",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chats",
                table: "Chats",
                columns: new[] { "UserA", "UserB" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatUserA_ChatUserB",
                table: "Messages",
                columns: new[] { "ChatUserA", "ChatUserB" });

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_ChatUserA_ChatUserB",
                table: "Messages",
                columns: new[] { "ChatUserA", "ChatUserB" },
                principalTable: "Chats",
                principalColumns: new[] { "UserA", "UserB" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
