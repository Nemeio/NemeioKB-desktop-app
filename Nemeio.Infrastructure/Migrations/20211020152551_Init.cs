using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nemeio.Infrastructure.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Language = table.Column<string>(nullable: true),
                    AugmentedImageEnable = table.Column<bool>(nullable: false),
                    ShowGrantPrivilegeWindow = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Blacklists",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    IsSystem = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blacklists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConfiguratorIndex = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Layouts",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Screen = table.Column<int>(nullable: false),
                    OsId = table.Column<string>(nullable: false),
                    AssociatedId = table.Column<string>(nullable: true),
                    Image = table.Column<byte[]>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    ConfiguratorIndex = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    DateCreation = table.Column<DateTime>(nullable: false),
                    DateUpdate = table.Column<DateTime>(nullable: false),
                    Enable = table.Column<bool>(nullable: false),
                    IsFactory = table.Column<bool>(nullable: false),
                    IsHid = table.Column<bool>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    Keys = table.Column<string>(nullable: true),
                    LinkAppPath = table.Column<string>(nullable: true),
                    LinkAppEnable = table.Column<bool>(nullable: false),
                    IsDarkMode = table.Column<bool>(nullable: false),
                    IsTemplate = table.Column<bool>(nullable: false),
                    FontName = table.Column<string>(nullable: true),
                    FontSize = table.Column<int>(nullable: false),
                    FontIsBold = table.Column<bool>(nullable: false),
                    FontIsItalic = table.Column<bool>(nullable: false),
                    ImageType = table.Column<int>(nullable: false),
                    AugmentedImageEnabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Layouts", x => new { x.Id, x.Screen });
                    table.ForeignKey(
                        name: "FK_Layouts_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ApplicationSettings",
                columns: new[] { "Id", "AugmentedImageEnable", "Language", "ShowGrantPrivilegeWindow" },
                values: new object[] { 1, true, null, true });

            migrationBuilder.InsertData(
                table: "Blacklists",
                columns: new[] { "Id", "IsSystem", "Name" },
                values: new object[] { 1, true, "nemeio" });

            migrationBuilder.InsertData(
                table: "Blacklists",
                columns: new[] { "Id", "IsSystem", "Name" },
                values: new object[] { 2, true, "explorer" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "ConfiguratorIndex", "IsDefault", "Title" },
                values: new object[] { 1, 0, true, "Default" });

            migrationBuilder.CreateIndex(
                name: "IX_Layouts_CategoryId",
                table: "Layouts",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationSettings");

            migrationBuilder.DropTable(
                name: "Blacklists");

            migrationBuilder.DropTable(
                name: "Layouts");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
