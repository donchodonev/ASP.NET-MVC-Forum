using Microsoft.EntityFrameworkCore.Migrations;

namespace ASP.NET_MVC_Forum.Data.Migrations
{
    public partial class PostModelPropertyNameFromNameToTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Posts",
                newName: "Title");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Posts",
                newName: "Name");
        }
    }
}
