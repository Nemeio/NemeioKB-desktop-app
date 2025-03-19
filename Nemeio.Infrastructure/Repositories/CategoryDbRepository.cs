using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Services.Category;
using Nemeio.Infrastructure.DbModels;

namespace Nemeio.Infrastructure.Repositories
{
    public class CategoryDbRepository : DbRepository, ICategoryDbRepository
    {
        public CategoryDbRepository(ILoggerFactory loggerFactory, IDatabaseAccessFactory dbAccessFactory)
            : base(loggerFactory.CreateLogger<CategoryDbRepository>(), dbAccessFactory) { }

        public IEnumerable<Category> ReadAllCategories()
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var data = dbAccess.DbContext.Categories.ToList();

                return data.Select(o => o.ToDomainModel()).ToList();
            }
        }

        public Category FindCategoryById(int id)
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var data = dbAccess.DbContext.Categories.First(o => o.Id == id);

                return data.ToDomainModel();
            }
        }

        public bool CategoryExists(int id)
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var exists = dbAccess.DbContext.Categories.Any(o => o.Id == id);

                return exists;
            }
        }

        public void DeleteCategory(int id)
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var data = dbAccess.DbContext.Categories.FirstOrDefault(o => o.Id == id);
                if (data != null)
                {
                    dbAccess.DbContext.Remove(data);
                    dbAccess.DbContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// IsDefault is always 'false' because there can be only one.
        /// The default category is created at database creation
        /// </summary>
        public int CreateCategory(Category cat)
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var data = new CategoryDbModel()
                {
                    ConfiguratorIndex = cat.Index,
                    Title = cat.Title,
                    IsDefault = false,
                };

                dbAccess.DbContext.Add(data);
                dbAccess.DbContext.SaveChanges();

                return data.Id;
            }
        }

        public void UpdateCategory(Category cat)
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var data = dbAccess.DbContext.Categories.FirstOrDefault(o => o.Id == cat.Id);
                if (data != null)
                {
                    data.ConfiguratorIndex = cat.Index;
                    data.Title = cat.Title;

                    dbAccess.DbContext.Update(data);
                    dbAccess.DbContext.SaveChanges();
                }
            }
        }
    }
}
