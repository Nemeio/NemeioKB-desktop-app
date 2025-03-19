using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.Services.Category;
using Nemeio.Infrastructure.Repositories;
using NUnit.Framework;

namespace Nemeio.Infrastructure.Test
{
    [TestFixture]
    public class CategoryDbRepositoryShould : DbRepositoryTestBase
    {
        private CategoryDbRepository _categoryDbRepository;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _categoryDbRepository = new CategoryDbRepository(new LoggerFactory(), _databaseAccessFactory);
        }

        [Test]
        public void CategoryDbRepository_FindCategoryById_WorksOk()
        {
            var category = new Category(NemeioConstants.DefaultCategoryId, 0, "Default", isDefault: true);
            _categoryDbRepository.CreateCategory(category);

            category = _categoryDbRepository.FindCategoryById(NemeioConstants.DefaultCategoryId);

            category.Should().NotBeNull();
        }

        [Test]
        public void CategoryDbRepository_CategoryExists_Found_WorksOk()
        {
            var category = new Category(NemeioConstants.DefaultCategoryId, 0, "Default", isDefault: true);
            _categoryDbRepository.CreateCategory(category);
         
            var exists = _categoryDbRepository.CategoryExists(NemeioConstants.DefaultCategoryId);

            exists.Should().Be(true);
        }

        [Test]
        public void CategoryDbRepository_CategoryExists_NotFound_WorksOk()
        {
            var exists = _categoryDbRepository.CategoryExists(0);

            exists.Should().Be(false);
        }

        [Test]
        public void CategoryDbRepository_DeleteCategory_WorksOk()
        {
            var category = new Category(NemeioConstants.DefaultCategoryId, 0, "Default", isDefault: true);
            _categoryDbRepository.CreateCategory(category);

            _categoryDbRepository.DeleteCategory(NemeioConstants.DefaultCategoryId);
        }

        [Test]
        public void CategoryDbRepository_ReadAllCategories_HasDefaultValue_WorksOk()
        {
            var categories = _categoryDbRepository.ReadAllCategories();

            categories.Count().Should().Be(1);
        }

        [Test]
        public void CategoryDbRepository_CreateCategory_TwoCategoriesPlusDefault_SaveOk()
        {
            var defaultCategory = new Category(NemeioConstants.DefaultCategoryId, 0, "Default", isDefault: true);
            var categoryOne = new Category(NemeioConstants.DefaultCategoryId + 1, 0, "Default title", isDefault: false);
            var categoryTwo = new Category(NemeioConstants.DefaultCategoryId + 2, 0, "second", isDefault: false);

            _categoryDbRepository.CreateCategory(categoryOne);
            _categoryDbRepository.CreateCategory(categoryTwo);

            var categories = _categoryDbRepository.ReadAllCategories();

            categories.Should().BeEquivalentTo(defaultCategory, categoryOne, categoryTwo);
        }

        [Test]
        public void CategoryDbRepository_UpdateCategory_WorksOk()
        {
            var category = new Category(NemeioConstants.DefaultCategoryId, 0, "Default", isDefault: true);
            _categoryDbRepository.CreateCategory(category);
            category = new Category(NemeioConstants.DefaultCategoryId, 1, "Updated", isDefault: true);
            _categoryDbRepository.UpdateCategory(category);

            category = _categoryDbRepository.FindCategoryById(NemeioConstants.DefaultCategoryId);

            category.Should().NotBeNull();
            category.Index.Should().Be(1);
            category.Title.Should().Be("Updated");
        }
    }
}
