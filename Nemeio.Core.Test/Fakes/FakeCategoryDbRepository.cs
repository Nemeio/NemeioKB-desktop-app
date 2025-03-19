using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Services.Category;

namespace Nemeio.Core.Test.Fakes
{
    public class FakeCategoryDbRepository : ICategoryDbRepository
    {
        public bool ReadAllCalled = false;
        public bool DeleteCalled = false;
        public bool FindOneByIdCalled = false;
        public bool SaveCalled = false;
        public bool UpdateCalled = false;

        private readonly List<Category> _categories = new List<Category>()
        {
            new Category(0, 0, "first"),
            new Category(1, 0, "second"),
            new Category(12, 0, "third"),
            new Category(123, 0, "fourth")
        };

        public List<Category> Categories => _categories;

        public void DeleteCategory(int categoryId)
        {
            DeleteCalled = true;

            var cat = _categories.First(x => x.Id == categoryId);

            _categories.Remove(cat);
        }

        public virtual Category FindCategoryById(int id)
        {
            FindOneByIdCalled = true;

            return _categories.First(x => x.Id == id);
        }

        public IEnumerable<Category> ReadAllCategories()
        {
            ReadAllCalled = true;

            return _categories;
        }

        public int CreateCategory(Category cat)
        {
            SaveCalled = true;

            _categories.Add(cat);

            return cat.Id;
        }

        public void UpdateCategory(Category category)
        {
            UpdateCalled = true;

            var cat = _categories.First(x => x.Id == category.Id);

            _categories.Remove(cat);
            _categories.Add(category);
        }

        public bool CategoryExists(int id)
        {
            return _categories.Count(x => x.Id == id) > 0;
        }
    }
}
