using System.Collections.Generic;
using Nemeio.Core.Keyboard.Screens;

namespace Nemeio.Core.Services.Layouts
{
    public interface ILayoutDbRepository
    {
        IEnumerable<LayoutId> ReadAllLayoutIds();

        IEnumerable<ILayout> ReadAllLayouts(ScreenType forScreen);

        IEnumerable<ILayout> ReadAllLayoutsWhereCategoryId(int id);

        ILayout ReadLayoutWithOsId(OsLayoutId osLayoutId);

        ILayout FindLayoutById(LayoutId id);

        int CountLayoutsForCategory(int categoryId);

        void UpdateLayout(ILayout layout);

        void CreateLayout(ILayout layout);

        void DeleteLayout(ILayout layout);

        void TransferLayoutOwnership(int fromCategoryId, int toCategoryId);

        IEnumerable<ILayout> GetTemplateLayouts();

        bool TemplateExists(string id);
    }
}
