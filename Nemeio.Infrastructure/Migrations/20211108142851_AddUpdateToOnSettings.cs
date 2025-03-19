using Microsoft.EntityFrameworkCore.Migrations;

namespace Nemeio.Infrastructure.Migrations
{
    public partial class AddUpdateToOnSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UpdateTo",
                table: "ApplicationSettings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdateTo",
                table: "ApplicationSettings");
        }
    }
}
