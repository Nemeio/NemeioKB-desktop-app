using Microsoft.EntityFrameworkCore.Migrations;

namespace Nemeio.Infrastructure.Migrations
{
    public partial class ReplaceHidDeletionByOriginalAssocatedId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE CopyLayouts AS SELECT * FROM Layouts
            ");
            migrationBuilder.DropTable("Layouts");
            migrationBuilder.Sql(@"
                CREATE TABLE Layouts AS SELECT Id, Screen, OsId, AssociatedId, Image, CategoryId, ConfiguratorIndex, Title, DateCreation, DateUpdate, Enable, IsFactory, IsHid, IsDefault, CopyLayouts.Keys, LinkAppPath, LinkAppEnable, IsTemplate, FontName, FontSize, FontIsBold, FontIsItalic, ImageType, AugmentedImageEnabled, XPositionAdjustment, YPositionAdjustment, Color, [Order], Subtitle, CASE WHEN IsHid = 0 THEN AssociatedId ELSE '' END AS OriginalAssociatedId FROM CopyLayouts
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
                CREATE TABLE Layouts AS SELECT Id, Screen, OsId, AssociatedId, Image, CategoryId, ConfiguratorIndex, Title, DateCreation, DateUpdate, Enable, IsFactory, IsDefault, CopyLayouts.Keys, LinkAppPath, LinkAppEnable, IsTemplate, FontName, FontSize, FontIsBold, FontIsItalic, ImageType, AugmentedImageEnabled, XPositionAdjustment, YPositionAdjustment, Color, [Order], Subtitle, CASE WHEN OriginalAssociatedId = '' 0 ELSE 1 END AS IsHid
            ");
            migrationBuilder.DropTable("CopyLayouts");
        }
    }
}
