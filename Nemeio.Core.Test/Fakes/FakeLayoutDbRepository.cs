using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Color;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Test.Fakes
{
    public class FakeLayoutDbRepository : ILayoutDbRepository
    {
        public static LayoutId FakeLayoutId1 = new LayoutId("8667DF98-1529-4A76-B846-78F5BFE57B2F");
        public static LayoutId FakeLayoutId2 = new LayoutId("8667DD98-1529-4A76-B846-78F5B1E57B4D");
        public static LayoutId FakeLayoutId3 = new LayoutId("8667DD98-1529-5894-B846-78F5B1E57B7B");
        public static LayoutId FakeLayoutId4 = new LayoutId("6357DD98-7859-5574-B846-78F5B1E57B2F");

        public static string FakeLayoutTitle1 = "Layout1";
        public static string FakeLayoutSubtitle1 = "Subtitle Layout1";
        public static string FakeLayoutTitle2 = "Layout2";
        public static string FakeLayoutSubtitle2 = "Subtitle Layout2";
        public static string FakeLayoutTitle3 = "Layout3";
        public static string FakeLayoutSubtitle3 = "Subtitle Layout3";
        public static string FakeLayoutTitle4 = "Layout4";
        public static string FakeLayoutSubtitle4 = "Subtitle Layout4";

        public bool SaveCalled = false;
        public bool ReadAllCalled = false;
        public bool UpdateCalled = false;
        public bool FindOneByIdCalled = false;
        public bool DeleteCalled = false;
        public ILayout LayoutToSave = null;

        public List<ILayout> Layouts = new List<ILayout>();

        public FakeLayoutDbRepository()
        {
            var keys = new List<Key>();
            
            for (int i = 0; i < 81; i++)
            {
                var newKey = new Key()
                {
                    Index = i,
                    Actions = new List<KeyAction>()
                    {
                        new KeyAction()
                        {
                            Display = "A",
                            Modifier = KeyboardModifier.None,
                            Subactions = new List<KeySubAction>()
                            {
                                new KeySubAction("A", KeyActionType.Unicode)
                            }
                        }
                    },
                    Disposition = KeyDisposition.Full
                };

                keys.Add(newKey);
            }

            var layoutImageInfo = new LayoutImageInfo(
                HexColor.Black,
                FontProvider.GetDefaultFont(),
                Mock.Of<IScreen>()
            );

            var layout1 = new Layout(
                new LayoutInfo(new OsLayoutId(""), false, false),
                layoutImageInfo,
                new byte[0],
                123,
                0,
                FakeLayoutTitle1,
                FakeLayoutSubtitle1,
                DateTime.Now,
                DateTime.Now,
                keys,
                FakeLayoutId1,
                null,
                false,
                true
            );
            var layout2 = new Layout(
                new LayoutInfo(new OsLayoutId(""), false, false),
                layoutImageInfo,
                new byte[0],
                12,
                0,
                FakeLayoutTitle2,
                FakeLayoutSubtitle2,
                DateTime.Now,
                DateTime.Now,
                keys,
                FakeLayoutId2,
                null,
                false,
                true
            );
            var layout3 = new Layout(
                new LayoutInfo(new OsLayoutId(""), false, false,  new List<string>() { @"C:\Programmes\Toto\toto.exe" }, true, true, true),
                layoutImageInfo,
                new byte[0],
                12,
                0,
                FakeLayoutTitle3,
                FakeLayoutSubtitle3,
                DateTime.Now,
                DateTime.Now,
                keys,
                FakeLayoutId3,
                null,
                false,
                true
            );
            var layout4 = new Layout(
                new LayoutInfo(new OsLayoutId(""), false, true, new List<string>(), false, true, true),
                layoutImageInfo,
                new byte[0],
                12,
                0,
                FakeLayoutTitle4,
                FakeLayoutSubtitle4,
                DateTime.Now,
                DateTime.Now,
                keys,
                FakeLayoutId4,
                null,
                false,
                true
            );
            Layouts = new List<ILayout>() { layout1, layout2, layout3, layout4 };
        }

        public void FakeSeedLayoutList(IList<ILayout> layoutList)
        {
            Layouts.Clear();
            Layouts.AddRange(layoutList);
        }

        //// Interface fake implementations

        public ILayout FindLayoutById(LayoutId id)
        {
            FindOneByIdCalled = true;

            return Layouts.First(x => x.LayoutId.ToString().Equals(id.ToString()));
        }

        public IEnumerable<ILayout> ReadAllLayouts(ScreenType screen)
        {
            ReadAllCalled = true;

            return Layouts;
        }

        public IEnumerable<ILayout> ReadAllLayoutsWhereCategoryId(int id) => new List<ILayout>();

        public ILayout ReadLayoutWithOsId(OsLayoutId osLayoutId) => throw new NotImplementedException();

        public void CreateLayout(ILayout layout)
        {
            if (layout == null)
            {
                throw new ArgumentNullException();
            }

            SaveCalled = true;
            LayoutToSave = layout;

            Layouts.Add(layout);
        }

        public void UpdateLayout(ILayout layout)
        {
            var found = Layouts.First(x => x.LayoutId == layout.LayoutId);
            UpdateCalled = true;
        }

        public int CountLayoutsForCategory(int categoryId) => Layouts.Count(x => x.CategoryId == categoryId);

        public void DeleteLayout(ILayout layout)
        {
            DeleteCalled = true;

            var found = Layouts.First(x => x.LayoutId == layout.LayoutId);

            Layouts.Remove(found);
        }

        public IEnumerable<LayoutId> ReadAllLayoutIds() => Layouts.Select(layout => layout.LayoutId).ToList();

        public void TransferLayoutOwnership(int fromCategoryId, int toCategoryId)
        {
            foreach (var layout in Layouts)
            {
                if (layout.CategoryId == fromCategoryId)
                {
                    layout.CategoryId = toCategoryId;
                }
            }
        }

        public IEnumerable<ILayout> GetTemplateLayouts()
        {
            return Layouts.Where(x => x.LayoutInfo.IsTemplate == true);
        }

        public bool TemplateExists(string id)
        {
            var layout = Layouts.FirstOrDefault(x => x.LayoutInfo.IsTemplate == true && x.LayoutId.Equals(new LayoutId(id)));
            
            return layout != null;
        }
    }
}
