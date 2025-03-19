using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nemeio.Infrastructure.Migrations
{
    public partial class ChangeDarkModeByColor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE CopyLayouts AS SELECT * FROM Layouts
            ");
            migrationBuilder.DropTable("Layouts");
            migrationBuilder.Sql(@"
                CREATE TABLE Layouts AS SELECT Id, Screen, OsId, AssociatedId, Image, CategoryId, ConfiguratorIndex, Title, DateCreation, DateUpdate, Enable, IsFactory, IsHid, IsDefault, CopyLayouts.Keys, LinkAppPath, LinkAppEnable, IsTemplate, FontName, FontSize, FontIsBold, FontIsItalic, ImageType, AugmentedImageEnabled, XPositionAdjustment, YPositionAdjustment, CASE WHEN IsDarkMode = 1 THEN '#000000' ELSE '#ffffff' END AS Color FROM CopyLayouts
            ");
            migrationBuilder.DropTable("CopyLayouts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE CopyLayouts AS SELECT * FROM Layouts
            ");
            migrationBuilder.DropTable("Layouts");
            migrationBuilder.Sql(@"
                CREATE TABLE Layouts AS SELECT Id, Screen, OsId, AssociatedId, Image, CategoryId, ConfiguratorIndex, Title, DateCreation, DateUpdate, Enable, IsFactory, IsHid, IsDefault, CopyLayouts.Keys, LinkAppPath, LinkAppEnable, IsTemplate, FontName, FontSize, FontIsBold, FontIsItalic, ImageType, AugmentedImageEnabled, XPositionAdjustment, YPositionAdjustment, CASE WHEN IsDarkMode = '#000000 THEN 1 ELSE 0 END AS IsDarkMode FROM CopyLayouts
            ");
            migrationBuilder.DropTable("CopyLayouts");
        }
    }
}
