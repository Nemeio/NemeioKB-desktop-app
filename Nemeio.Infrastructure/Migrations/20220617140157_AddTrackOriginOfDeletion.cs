using Microsoft.EntityFrameworkCore.Migrations;

namespace Nemeio.Infrastructure.Migrations
{
    public partial class AddTrackOriginOfDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RemovedByHidDeletion",
                table: "Layouts",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemovedByHidDeletion",
                table: "Layouts");
        }
    }
}
