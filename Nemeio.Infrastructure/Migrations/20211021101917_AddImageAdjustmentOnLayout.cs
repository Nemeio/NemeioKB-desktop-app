using Microsoft.EntityFrameworkCore.Migrations;

namespace Nemeio.Infrastructure.Migrations
{
    public partial class AddImageAdjustmentOnLayout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "XPositionAdjustment",
                table: "Layouts",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "YPositionAdjustment",
                table: "Layouts",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "XPositionAdjustment",
                table: "Layouts");

            migrationBuilder.DropColumn(
                name: "YPositionAdjustment",
                table: "Layouts");
        }
    }
}
