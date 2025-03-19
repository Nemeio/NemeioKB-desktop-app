using System.Collections.Generic;

namespace Nemeio.Core.Services.Category
{
    public interface ICategoryDbRepository
    {
        IEnumerable<Category> ReadAllCategories();

        Category FindCategoryById(int id);

        void DeleteCategory(int id);

        int CreateCategory(Category layout);

        void UpdateCategory(Category cat);

        bool CategoryExists(int id);
    }
}
