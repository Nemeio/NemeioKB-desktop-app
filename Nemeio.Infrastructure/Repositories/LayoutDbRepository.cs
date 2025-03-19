using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Services.Layouts;
using Nemeio.Infrastructure.DbModels;
using Newtonsoft.Json;

namespace Nemeio.Infrastructure
{
    public class LayoutDbRepository : DbRepository, ILayoutDbRepository
    {
        public const char LinkDelimiter = ';';

        private readonly IScreenFactory _screenFactory;

        public LayoutDbRepository(ILoggerFactory loggerFactory, IDatabaseAccessFactory dbAccessFactory, IScreenFactory screenFactory)
            : base(loggerFactory.CreateLogger<LayoutDbRepository>(), dbAccessFactory)
        {
            _screenFactory = screenFactory ?? throw new ArgumentNullException(nameof(screenFactory));
        }

        public IEnumerable<LayoutId> ReadAllLayoutIds()
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var ids = dbAccess.DbContext.Layouts.Select(o => o.Id).ToList();

                return ids.Select(o => new LayoutId(o)).ToList();
            }
        }

        public IEnumerable<ILayout> ReadAllLayouts(ScreenType forScreen)
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var layouts = dbAccess
                    .DbContext
                    .Layouts
                    .Where(layout => layout.Screen == forScreen)
                    .ToList();

                return layouts
                    .Select(o => o.ToDomainModel(_screenFactory))
                    .ToList();
            }
        }

        public ILayout ReadLayoutWithOsId(OsLayoutId osLayoutId)
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var data = dbAccess.DbContext.Layouts.FirstOrDefault(o => o.OsId == osLayoutId.Id);

                return data?.ToDomainModel(_screenFactory);
            }
        }

        public IEnumerable<ILayout> ReadAllLayoutsWhereCategoryId(int id)
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var data = dbAccess.DbContext.Layouts.Where(o => o.CategoryId == id).ToList();

                return data.Select(o => o.ToDomainModel(_screenFactory)).ToList();
            }
        }

        public ILayout FindLayoutById(LayoutId id)
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var data = dbAccess.DbContext.Layouts.First(o => o.Id == id);

                return data.ToDomainModel(_screenFactory);
            }
        }

        public int CountLayoutsForCategory(int categoryId)
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var count = dbAccess.DbContext.Layouts.Count(o => o.CategoryId == categoryId);

                return count;
            }
        }

        public IEnumerable<ILayout> GetTemplateLayouts()
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var data = dbAccess.DbContext.Layouts.Where(o => o.IsTemplate);

                return data.Select(o => o.ToDomainModel(_screenFactory)).ToList();
            }
        }

        public bool TemplateExists(string id)
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var exists = dbAccess.DbContext.Layouts.Any(o => o.IsTemplate && o.Id == id);

                return exists;
            }
        }

        public void UpdateLayout(ILayout layout)
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var links = string.Empty;

                if (layout.LayoutInfo.LinkApplicationPaths.Any())
                {
                    links = layout.LayoutInfo
                                .LinkApplicationPaths
                                .Aggregate((i, j) => $"{i.ToLower()}{LinkDelimiter}{j}");
                }

                var data = dbAccess.DbContext.Layouts.FirstOrDefault(o => o.Id == layout.LayoutId);
                if (data != null)
                {
                    data.Id = layout.LayoutId;
                    data.Screen = layout.LayoutImageInfo.Screen.Type;
                    data.AssociatedId = layout.AssociatedLayoutId;
                    data.Image = layout.Image;
                    data.ConfiguratorIndex = layout.Index;
                    data.Title = layout.Title;
                    data.Subtitle = layout.Subtitle;
                    data.DateUpdate = layout.DateUpdated;
                    data.Enable = layout.Enable;
                    data.CategoryId = layout.CategoryId;
                    data.IsHid = layout.LayoutInfo.Hid;
                    data.IsFactory = layout.LayoutInfo.Factory;
                    data.IsDefault = layout.IsDefault;
                    data.Keys = JsonConvert.SerializeObject(layout.Keys);
                    data.LinkAppPath = links;
                    data.LinkAppEnable = layout.LayoutInfo.LinkApplicationEnable;
                    data.Color = layout.LayoutImageInfo.Color;
                    data.IsTemplate = layout.LayoutInfo.IsTemplate;
                    data.FontName = layout.LayoutImageInfo.Font.Name;
                    data.FontSize = layout.LayoutImageInfo.Font.Size;
                    data.FontIsBold = layout.LayoutImageInfo.Font.Bold;
                    data.FontIsItalic = layout.LayoutImageInfo.Font.Italic;
                    data.AugmentedImageEnabled = layout.LayoutInfo.AugmentedHidEnable;
                    data.ImageType = layout.LayoutImageInfo.ImageType;
                    data.XPositionAdjustment = layout.LayoutImageInfo.XPositionAdjustment;
                    data.YPositionAdjustment = layout.LayoutImageInfo.YPositionAdjustement;
                    data.Order = layout.Order;
                    data.AssociatedId = layout.AssociatedLayoutId;
                    data.OriginalAssociatedId = layout.OriginalAssociatedLayoutId;
                    dbAccess.DbContext.Update(data);
                    dbAccess.DbContext.SaveChanges();
                }
            }
        }

        public void CreateLayout(ILayout layout)
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var links = string.Empty;

                if (layout.LayoutInfo.LinkApplicationPaths.Any())
                {
                    links = layout.LayoutInfo
                                .LinkApplicationPaths
                                .Aggregate((i, j) => $"{i.ToLower()}{LinkDelimiter}{j}");
                }

                var data = new LayoutDbModel()
                {
                    Id = layout.LayoutId,
                    Screen = layout.LayoutImageInfo.Screen.Type,
                    OsId = layout.LayoutInfo.OsLayoutId,
                    AssociatedId = layout.AssociatedLayoutId,
                    Image = layout.Image,
                    ConfiguratorIndex = layout.Index,
                    Title = layout.Title,
                    Subtitle = layout.Subtitle,
                    DateCreation = layout.DateCreated,
                    DateUpdate = layout.DateUpdated,
                    Enable = layout.Enable,
                    CategoryId = layout.CategoryId,
                    IsHid = layout.LayoutInfo.Hid,
                    IsFactory = layout.LayoutInfo.Factory,
                    IsDefault = layout.IsDefault,
                    Keys = JsonConvert.SerializeObject(layout.Keys),
                    LinkAppPath = links,
                    LinkAppEnable = layout.LayoutInfo.LinkApplicationEnable,
                    Color = layout.LayoutImageInfo.Color,
                    IsTemplate = layout.LayoutInfo.IsTemplate,
                    FontName = layout.LayoutImageInfo.Font.Name,
                    FontSize = layout.LayoutImageInfo.Font.Size,
                    FontIsBold = layout.LayoutImageInfo.Font.Bold,
                    FontIsItalic = layout.LayoutImageInfo.Font.Italic,
                    AugmentedImageEnabled = layout.LayoutInfo.AugmentedHidEnable,
                    XPositionAdjustment = layout.LayoutImageInfo.XPositionAdjustment,
                    YPositionAdjustment = layout.LayoutImageInfo.YPositionAdjustement,
                    OriginalAssociatedId = layout.OriginalAssociatedLayoutId
                };

                dbAccess.DbContext.Add(data);
                dbAccess.DbContext.SaveChanges();
            }
        }

        public void DeleteLayout(ILayout layout)
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var data = dbAccess.DbContext.Layouts.First(o => o.Id == layout.LayoutId);

                dbAccess.DbContext.Remove(data);
                dbAccess.DbContext.SaveChanges();
            }
        }

        public void TransferLayoutOwnership(int fromCategoryId, int toCategoryId)
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var categories = dbAccess.DbContext.Layouts.Where(o => o.CategoryId == fromCategoryId);
                foreach (var category in categories)
                {
                    category.CategoryId = toCategoryId;
                    dbAccess.DbContext.Update(category);
                }

                dbAccess.DbContext.SaveChanges();
            }
        }
    }
}
