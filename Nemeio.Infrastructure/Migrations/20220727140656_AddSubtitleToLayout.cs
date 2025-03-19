using Microsoft.EntityFrameworkCore.Migrations;

namespace Nemeio.Infrastructure.Migrations
{
    public partial class AddSubtitleToLayout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subtitle",
                table: "Layouts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subtitle",
                table: "Layouts");
        }
    }
}
