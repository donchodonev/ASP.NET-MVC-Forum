using Microsoft.EntityFrameworkCore.Migrations;

namespace ASP.NET_MVC_Forum.Data.Migrations
{
    public partial class RemoveImageUrlFromPosts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Posts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
