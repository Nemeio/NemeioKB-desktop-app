using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Images.Builders;
using Nemeio.Core.Images.Renderer;
using Nemeio.Core.Keyboard.Map;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Color;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services.Category;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Settings;
using Nemeio.Infrastructure.Repositories;
using NUnit.Framework;

namespace Nemeio.Infrastructure.Test
{
    [TestFixture]
    public class LayoutDbRepositoryShould : DbRepositoryTestBase
    {
        private CategoryDbRepository _categoryDbRepository;
        private LayoutDbRepository _layoutDbRepository;
        private IScreenFactory _screenFactory;
        private KeyboardMap _fakeKeyboardMap;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _fakeKeyboardMap = new KeyboardMap(0, 0, new List<KeyboardButton>(), false);

            var mockKeyboardMapFactory = Mock.Of<IKeyboardMapFactory>();
            Mock.Get(mockKeyboardMapFactory)
                .Setup(x => x.CreateEinkMap())
                .Returns(_fakeKeyboardMap);
            Mock.Get(mockKeyboardMapFactory)
                .Setup(x => x.CreateHolitechMap())
                .Returns(_fakeKeyboardMap);

            var mockSettingsHolder = Mock.Of<ISettingsHolder>();
            var mockJpegRenderer = Mock.Of<IJpegRenderer>();
            var mockOneBppRenderer = Mock.Of<IOneBppRenderer>();

            _screenFactory = new ScreenFactory(mockKeyboardMapFactory, mockJpegRenderer, mockOneBppRenderer, mockSettingsHolder);

            _categoryDbRepository = new CategoryDbRepository(new LoggerFactory(), _databaseAccessFactory);
            _layoutDbRepository = new LayoutDbRepository(new LoggerFactory(), _databaseAccessFactory, _screenFactory);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void LayoutDbRepository_ReadAllLayoutIds_Ok(int count)
        {
            var screen = Mock.Of<IScreen>();
            Mock.Get(screen)
                .Setup(x => x.Type)
                .Returns(ScreenType.Eink);

            for (int i = 0; i < count; i++)
            {
                var layout = new Layout(new LayoutInfo(new OsLayoutId($"fr-FR-{i}"), false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { (byte)i, 255, 128, 064, 025, 004, 019 }, NemeioConstants.DefaultCategoryId, 0, "lyt1", "sublyt1", DateTime.Now, DateTime.Now, new List<Key>());
                _layoutDbRepository.CreateLayout(layout);
            }
            var layoutIds = _layoutDbRepository.ReadAllLayoutIds().ToList();

            layoutIds.Should().HaveCount(count);
        }

        [Test]
        public void LayoutDbRepository_ReadLayoutWithOsId_Ok()
        {
            var osLayoutId = new OsLayoutId("fr-FR");
            var screen = Mock.Of<IScreen>();
            Mock.Get(screen)
                .Setup(x => x.Type)
                .Returns(ScreenType.Eink);

            ILayout layout = new Layout(new LayoutInfo(osLayoutId, false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 001, 255, 128, 064, 025, 004, 019 }, NemeioConstants.DefaultCategoryId, NemeioConstants.DefaultCategoryId, "lyt1", "sublyt1", DateTime.Now, DateTime.Now, new List<Key>());
            _layoutDbRepository.CreateLayout(layout);

            layout = _layoutDbRepository.ReadLayoutWithOsId(osLayoutId);

            layout.Should().NotBeNull();
        }

        [Test]
        public void LayoutDbRepository_ReadLayoutWithOsId_NotFound()
        {
            var osLayoutId = new OsLayoutId("notFound");

            var layout = _layoutDbRepository.ReadLayoutWithOsId(osLayoutId);

            layout.Should().BeNull();
        }

        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        public void LayoutDbRepository_ReadAllLayoutsWhereCategoryId_Ok(int categoryId, int count)
        {
            var screen = Mock.Of<IScreen>();
            Mock.Get(screen)
                .Setup(x => x.Type)
                .Returns(ScreenType.Eink);

            _categoryDbRepository.CreateCategory(new Category(1, "cat1"));
            _categoryDbRepository.CreateCategory(new Category(1, "cat2"));
            _layoutDbRepository.CreateLayout(new Layout(new LayoutInfo(new OsLayoutId("fr-FR"), false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 001, 255, 128, 064, 025, 004, 019 }, 1, 0, "lyt1", "lyt1", DateTime.Now, DateTime.Now, new List<Key>()));
            _layoutDbRepository.CreateLayout(new Layout(new LayoutInfo(new OsLayoutId("fr-FR-2"), false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 002, 255, 128, 064, 025, 004, 019 }, 2, 0, "lyt1", "lyt1", DateTime.Now, DateTime.Now, new List<Key>()));
            _layoutDbRepository.CreateLayout(new Layout(new LayoutInfo(new OsLayoutId("fr-FR-3"), false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 003, 255, 128, 064, 025, 004, 019 }, 2, 0, "lyt1", "lyt1", DateTime.Now, DateTime.Now, new List<Key>()));

            var layouts = _layoutDbRepository.ReadAllLayoutsWhereCategoryId(categoryId);

            layouts.Should().NotBeNull();
            layouts.Should().HaveCount(count);
        }

        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        public void LayoutDbRepository_CountLayoutsForCategory_Ok(int categoryId, int count)
        {
            var screen = Mock.Of<IScreen>();
            Mock.Get(screen)
                .Setup(x => x.Type)
                .Returns(ScreenType.Eink);

            _categoryDbRepository.CreateCategory(new Category(1, "cat1"));
            _categoryDbRepository.CreateCategory(new Category(1, "cat2"));
            _layoutDbRepository.CreateLayout(new Layout(new LayoutInfo(new OsLayoutId("fr-FR"), false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 001, 255, 128, 064, 025, 004, 019 }, 1, 0, "lyt1", "sublyt1", DateTime.Now, DateTime.Now, new List<Key>()));
            _layoutDbRepository.CreateLayout(new Layout(new LayoutInfo(new OsLayoutId("fr-FR-2"), false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 002, 255, 128, 064, 025, 004, 019 }, 2, 0, "lyt1", "sublyt1", DateTime.Now, DateTime.Now, new List<Key>()));
            _layoutDbRepository.CreateLayout(new Layout(new LayoutInfo(new OsLayoutId("fr-FR-3"), false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 003, 255, 128, 064, 025, 004, 019 }, 2, 0, "lyt1", "sublyt1", DateTime.Now, DateTime.Now, new List<Key>()));

            var layoutsFound = _layoutDbRepository.CountLayoutsForCategory(categoryId);

            layoutsFound.Should().Be(count);
        }

        [Test]
        public void LayoutDbRepository_GetTemplateLayouts_Ok()
        {
            var screen = Mock.Of<IScreen>();
            Mock.Get(screen)
                .Setup(x => x.Type)
                .Returns(ScreenType.Eink);

            _categoryDbRepository.CreateCategory(new Category(1, "cat1"));
            _categoryDbRepository.CreateCategory(new Category(1, "cat2"));
            _layoutDbRepository.CreateLayout(new Layout(new LayoutInfo(new OsLayoutId("fr-FR"), false, false, isTemplate: false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 001, 255, 128, 064, 025, 004, 019 }, 1, 0, "lyt1", "sublyt1", DateTime.Now, DateTime.Now, new List<Key>()));
            _layoutDbRepository.CreateLayout(new Layout(new LayoutInfo(new OsLayoutId("fr-FR-2"), false, false, isTemplate: true), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 002, 255, 128, 064, 025, 004, 019 }, 2, 0, "lyt1", "sublyt1", DateTime.Now, DateTime.Now, new List<Key>()));
            _layoutDbRepository.CreateLayout(new Layout(new LayoutInfo(new OsLayoutId("fr-FR-3"), false, false, isTemplate: true), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 003, 255, 128, 064, 025, 004, 019 }, 2, 0, "lyt1", "sublyt1", DateTime.Now, DateTime.Now, new List<Key>()));

            var layouts = _layoutDbRepository.GetTemplateLayouts();

            layouts.Should().NotBeNull();
            layouts.Should().HaveCount(2);
        }

        [Test]
        public void LayoutDbRepository_FindLayoutById_Ok()
        {
            var screen = Mock.Of<IScreen>();
            Mock.Get(screen)
                .Setup(x => x.Type)
                .Returns(ScreenType.Eink);

            ILayout layout = new Layout(new LayoutInfo(new OsLayoutId("fr-FR"), false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 001, 255, 128, 064, 025, 004, 019 }, NemeioConstants.DefaultCategoryId, 0, "lyt1", "sublyt1", DateTime.Now, DateTime.Now, new List<Key>());
            _layoutDbRepository.CreateLayout(layout);

            layout = _layoutDbRepository.FindLayoutById(layout.LayoutId);

            layout.Should().NotBeNull();
        }

        [Test]
        public void LayoutDbRepository_FindLayoutById_NotFound()
        {
            TestDelegate action = () => _layoutDbRepository.FindLayoutById(LayoutId.NewLayoutId);

            Assert.Throws<InvalidOperationException>(action);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void LayoutDbRepository_TemplateExists_Ok(bool isTemplate)
        {
            var screen = Mock.Of<IScreen>();
            Mock.Get(screen)
                .Setup(x => x.Type)
                .Returns(ScreenType.Eink);

            var layout = new Layout(new LayoutInfo(new OsLayoutId("fr-FR"), false, false, isTemplate: isTemplate), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 001, 255, 128, 064, 025, 004, 019 }, NemeioConstants.DefaultCategoryId, 0, "lyt1", "sublyt1", DateTime.Now, DateTime.Now, new List<Key>());
            _layoutDbRepository.CreateLayout(layout);

            var exists = _layoutDbRepository.TemplateExists(layout.LayoutId);

            exists.Should().Be(isTemplate);
        }

        [Test]
        public void LayoutDbRepository_CreateLayout_Ok()
        {
            var screenType = ScreenType.Holitech;
            var screen = _screenFactory.CreateScreen(screenType);

            var layout1 = new Layout(new LayoutInfo(new OsLayoutId("fr-FR"), false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 001, 255, 128, 064, 025, 004, 019 }, NemeioConstants.DefaultCategoryId, 0, "lyt1", "sublyt1", DateTime.Now, DateTime.Now, new List<Key>());
            var layout2 = new Layout(new LayoutInfo(new OsLayoutId("ar-TN"), false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 101, 000, 032, 096, 011, 002, 019 }, NemeioConstants.DefaultCategoryId, 0, "lyt2", "sublyt2", DateTime.Now, DateTime.Now, new List<Key>());

            _layoutDbRepository.CreateLayout(layout1);
            _layoutDbRepository.CreateLayout(layout2);

            var layouts = _layoutDbRepository.ReadAllLayouts(screenType);

            layouts.Should().BeEquivalentTo(layout1, layout2);
        }

        [Test]
        public void LayoutDbRepository_UpdateLayout_OneLayoutOutOfTwo_Ok()
        {
            var screenType = ScreenType.Holitech;
            var screen = _screenFactory.CreateScreen(screenType);

            // create a layout and save
            var layout1 = new Layout(new LayoutInfo(new OsLayoutId("fr-FR"), false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 001, 255, 128, 064, 025, 004, 019 }, NemeioConstants.DefaultCategoryId, 0, "lyt1", "sublyt1", DateTime.Now, DateTime.Now, new List<Key>());
            var layout2 = new Layout(new LayoutInfo(new OsLayoutId("ar-TN"), false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 101, 000, 032, 096, 011, 002, 019 }, NemeioConstants.DefaultCategoryId, 0, "lyt2", "sublyt2", DateTime.Now, DateTime.Now, new List<Key>());
            _layoutDbRepository.CreateLayout(layout1);
            _layoutDbRepository.CreateLayout(layout2);

            // updated it by setting as default
            layout1.LayoutInfo.Factory = true;
            _layoutDbRepository.UpdateLayout(layout1);

            // check new status validity
            var layouts = _layoutDbRepository.ReadAllLayouts(screenType);

            layouts.Should().BeEquivalentTo(layout1, layout2);
        }

        [Test]
        public void LayoutDbRepository_UpdateLayout_ImageTypeIsSaved_Ok()
        {
            var updateImageType = LayoutImageType.Hide;
            var screenType = ScreenType.Holitech;
            var screen = Mock.Of<IScreen>();
            Mock.Get(screen)
                .Setup(x => x.Type)
                .Returns(screenType);

            var myLayout = new Layout(new LayoutInfo(new OsLayoutId("ar-TN"), false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 101, 000, 032, 096, 011, 002, 019 }, NemeioConstants.DefaultCategoryId, 0, "lyt2", "sublyt2", DateTime.Now, DateTime.Now, new List<Key>());

            _layoutDbRepository.CreateLayout(myLayout);

            myLayout.LayoutImageInfo.ImageType = updateImageType;

            _layoutDbRepository.UpdateLayout(myLayout);

            var myUpdatedLayout = _layoutDbRepository
                .ReadAllLayouts(screenType)
                .First(x => x.LayoutId.Equals(myLayout.LayoutId));

            myUpdatedLayout.LayoutImageInfo.ImageType.Should().Be(updateImageType);
        }

        [Test]
        public void LayoutDbRepository_DeleteLayout_Ok()
        {
            var screen = Mock.Of<IScreen>();
            Mock.Get(screen)
                .Setup(x => x.Type)
                .Returns(ScreenType.Eink);

            var layout = new Layout(new LayoutInfo(new OsLayoutId("fr-FR"), false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 001, 255, 128, 064, 025, 004, 019 }, NemeioConstants.DefaultCategoryId, 0, "lyt1", "sublyt1", DateTime.Now, DateTime.Now, new List<Key>());
            _layoutDbRepository.CreateLayout(layout);

            _layoutDbRepository.DeleteLayout(layout);
            TestDelegate action = () => _layoutDbRepository.FindLayoutById(layout.LayoutId);

            Assert.Throws<InvalidOperationException>(action);
        }

        [Test]
        public void LayoutDbRepository_DeleteLayout_NotFound()
        {
            var screen = Mock.Of<IScreen>();
            Mock.Get(screen)
                .Setup(x => x.Type)
                .Returns(ScreenType.Eink);

            var layout = new Layout(new LayoutInfo(new OsLayoutId("fr-FR"), false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 001, 255, 128, 064, 025, 004, 019 }, NemeioConstants.DefaultCategoryId, 0, "lyt1", "sublyt1", DateTime.Now, DateTime.Now, new List<Key>());
            _layoutDbRepository.CreateLayout(layout);

            _layoutDbRepository.DeleteLayout(layout);

            TestDelegate action = () => _layoutDbRepository.DeleteLayout(layout);

            Assert.Throws<InvalidOperationException>(action);
        }

        [Test]
        public void LayoutDbRepository_TransferLayoutOwnership_Ok()
        {
            var screen = Mock.Of<IScreen>();
            Mock.Get(screen)
                .Setup(x => x.Type)
                .Returns(ScreenType.Eink);

            var categoryId1 = _categoryDbRepository.CreateCategory(new Category(1, "cat1"));
            var categoryId2 = _categoryDbRepository.CreateCategory(new Category(1, "cat1"));
            _categoryDbRepository.CreateCategory(new Category(1, "cat2"));
            _layoutDbRepository.CreateLayout(new Layout(new LayoutInfo(new OsLayoutId("fr-FR"), false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 001, 255, 128, 064, 025, 004, 019 }, categoryId1, 0, "lyt1", "sublyt1", DateTime.Now, DateTime.Now, new List<Key>()));
            _layoutDbRepository.CreateLayout(new Layout(new LayoutInfo(new OsLayoutId("fr-FR-2"), false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 002, 255, 128, 064, 025, 004, 019 }, categoryId2, 0, "lyt1", "sublyt1", DateTime.Now, DateTime.Now, new List<Key>()));
            _layoutDbRepository.CreateLayout(new Layout(new LayoutInfo(new OsLayoutId("fr-FR-3"), false, false), new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen), new byte[] { 003, 255, 128, 064, 025, 004, 019 }, categoryId2, 0, "lyt1", "sublyt1", DateTime.Now, DateTime.Now, new List<Key>()));

            _layoutDbRepository.TransferLayoutOwnership(categoryId1, categoryId2);
            var count = _layoutDbRepository.CountLayoutsForCategory(categoryId2);

            count.Should().Be(3);
        }
    }
}
