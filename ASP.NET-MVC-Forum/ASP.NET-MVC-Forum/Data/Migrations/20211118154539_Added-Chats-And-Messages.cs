using Microsoft.EntityFrameworkCore.Migrations;

namespace ASP.NET_MVC_Forum.Data.Migrations
{
    public partial class AddedChatsAndMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    UserA = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserB = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => new { x.UserA, x.UserB });
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: true),
                    ChatUserA = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ChatUserB = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_ChatUserA_ChatUserB",
                        columns: x => new { x.ChatUserA, x.ChatUserB },
                        principalTable: "Chats",
                        principalColumns: new[] { "UserA", "UserB" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatUserA_ChatUserB",
                table: "Messages",
                columns: new[] { "ChatUserA", "ChatUserB" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Chats");
        }
    }
}
