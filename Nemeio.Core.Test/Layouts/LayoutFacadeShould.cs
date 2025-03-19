using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.Core.Images;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts;
using Nemeio.Core.Layouts.Color;
using Nemeio.Core.Layouts.Export;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Layouts.Images.AugmentedLayout;
using Nemeio.Core.Layouts.Import;
using Nemeio.Core.Layouts.Synchronization;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services.Layouts;
using NUnit.Framework;

namespace Nemeio.Core.Test.Layouts
{
    [TestFixture]
    public class LayoutFacadeShould
    {
        private ILayoutLibrary _library;
        private ILayoutImporter _importer;
        private ILayoutExportService _exporter;
        private ILayoutImageGenerator _genService;
        private ISynchronizer _synchronizer;
        private IAugmentedLayoutImageProvider _augmentedLayoutImageProvider;
        private ILayoutFactory _factory;
        private LayoutFacade _facade;
        private ILayout _layout;

        [SetUp]
        public void SetUp()
        {
            _library = Mock.Of<ILayoutLibrary>();
            _importer = Mock.Of<ILayoutImporter>();
            _exporter = Mock.Of<ILayoutExportService>();
            _genService = Mock.Of<ILayoutImageGenerator>();
            _synchronizer = Mock.Of<ISynchronizer>();
            _augmentedLayoutImageProvider = Mock.Of<IAugmentedLayoutImageProvider>();
            _factory = Mock.Of<ILayoutFactory>();
            _layout = Mock.Of<ILayout>();

            _facade = new LayoutFacade(_library, _factory, _synchronizer, _importer, _exporter, _genService, _augmentedLayoutImageProvider);
        }

        [Test]
        public void LayoutFacade_AddLayoutAsync_WithNullParameter_Throws()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _facade.AddLayoutAsync(null));
        }

        [Test]
        public async Task LayoutFacade_AddLayoutAsync_Ok()
        {
            var libraryAddCalled = false;
            var synchronizeCalled = false;

            Mock.Get(_library)
                .Setup(x => x.AddLayoutAsync(It.IsAny<ILayout>()))
                .ReturnsAsync(() => _layout)
                .Callback(() => libraryAddCalled = true);

            Mock.Get(_synchronizer)
                .Setup(x => x.SynchronizeAsync())
                .Returns(Task.Delay(1))
                .Callback(() => synchronizeCalled = true);

            await _facade.AddLayoutAsync(_layout);

            libraryAddCalled.Should().BeTrue();
            synchronizeCalled.Should().BeTrue();
        }

        [Test]
        public void LayoutFacade_RemoveLayoutAsync_WithNullParameter_Throws()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _facade.RemoveLayoutAsync(layout: null));
        }

        [Test]
        public async Task LayoutFacade_RemoveLayoutAsync_Ok()
        {
            var libraryRemoveCalled = false;
            var synchronizeCalled = false;

            Mock.Get(_library)
                .Setup(x => x.RemoveLayoutAsync(It.IsAny<ILayout>()))
                .Returns(Task.Delay(1))
                .Callback(() => libraryRemoveCalled = true);

            Mock.Get(_synchronizer)
                .Setup(x => x.SynchronizeAsync())
                .Returns(Task.Delay(1))
                .Callback(() => synchronizeCalled = true);

            await _facade.RemoveLayoutAsync(_layout);

            libraryRemoveCalled.Should().BeTrue();
            synchronizeCalled.Should().BeTrue();
        }

        [Test]
        public void LayoutFacade_UpdateLayoutAsync_WithNullParameter_Throws()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _facade.UpdateLayoutAsync(null));
        }

        [Test]
        public async Task LayoutFacade_UpdateLayoutAsync_Ok()
        {
            var libraryUpdateCalled = false;
            var synchronizeCalled = false;

            Mock.Get(_library)
                .Setup(x => x.UpdateLayoutAsync(It.IsAny<ILayout>()))
                .ReturnsAsync(() => _layout)
                .Callback(() => libraryUpdateCalled = true);

            Mock.Get(_synchronizer)
                .Setup(x => x.SynchronizeAsync())
                .Returns(Task.Delay(1))
                .Callback(() => synchronizeCalled = true);

            await _facade.UpdateLayoutAsync(_layout);

            libraryUpdateCalled.Should().BeTrue();
            synchronizeCalled.Should().BeTrue();
        }

        [Test]
        public void LayoutFacade_DuplicateLayoutAsync_WithNullParameter_Throws()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _facade.DuplicateLayoutAsync(null, string.Empty));
        }

        [Test]
        public async Task LayoutFacade_DuplicateLayoutAsync_Ok()
        {
            const string duplicateLayoutTitle = "myDuplicateLayoutTitle";

            var libraryAddCalled = false;
            var synchronizeCalled = false;

            Mock.Get(_factory)
                .Setup(x => x.Duplicate(It.IsAny<ILayout>(), It.IsAny<string>()))
                .Returns(_layout);

            Mock.Get(_library)
                .Setup(x => x.AddLayoutAsync(It.IsAny<ILayout>()))
                .ReturnsAsync(() => _layout)
                .Callback(() => libraryAddCalled = true);

            Mock.Get(_synchronizer)
                .Setup(x => x.SynchronizeAsync())
                .Returns(Task.Delay(1))
                .Callback(() => synchronizeCalled = true);

           var duplicateLayout = await _facade.DuplicateLayoutAsync(_layout, duplicateLayoutTitle);

            duplicateLayout.Should().NotBeNull();
            libraryAddCalled.Should().BeTrue();
            synchronizeCalled.Should().BeTrue();
        }

        [Test]
        public void LayoutFacade_ImportLayoutAsync_WithNullParameter_Throws()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _facade.ImportLayoutAsync(null));
        }

        [Test]
        public async Task LayoutFacade_ImportLayoutAsync_Ok()
        {
            var libraryAddCalled = false;
            var synchronizeCalled = false;

            Mock.Get(_importer)
                .Setup(x => x.ImportLayout(It.IsAny<LayoutExport>()))
                .Returns(_layout);

            Mock.Get(_library)
                .Setup(x => x.AddLayoutAsync(It.IsAny<ILayout>()))
                .ReturnsAsync(() => _layout)
                .Callback(() => libraryAddCalled = true);

            Mock.Get(_synchronizer)
                .Setup(x => x.SynchronizeAsync())
                .Returns(Task.Delay(1))
                .Callback(() => synchronizeCalled = true);

            var dummyExport = new LayoutExport();
            var importedLayout = await _facade.ImportLayoutAsync(dummyExport);

            importedLayout.Should().NotBeNull();
            libraryAddCalled.Should().BeTrue();
            synchronizeCalled.Should().BeTrue();
        }

        [Test]
        public void LayoutFacade_ExportLayoutAsync_WithNullParameter_Throws()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _facade.ExportLayoutAsync(null));
        }

        [Test]
        public async Task LayoutFacade_RefreshAugmentedLayoutAsync_Ok()
        {
            var libraryUpdateCalled = false;
            var libraryUpdateCountCalled = 0;
            var synchronizeCalled = false;

            var keys = new List<Key>()
            {
                new Key()
                {
                    Index = 0,
                    Font = FontProvider.GetDefaultFont(),
                    Edited = false,
                    Disposition = KeyDisposition.Full,
                    Actions = new List<KeyAction>()
                }
            };

            var screen = Mock.Of<IScreen>();

            var frenchHidLayout = Mock.Of<ILayout>();
            Mock.Get(frenchHidLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(new OsLayoutId("french"), false, true, null, false, true, true));
            Mock.Get(frenchHidLayout)
                .Setup(x => x.LayoutImageInfo)
                .Returns(new LayoutImageInfo(HexColor.White, FontProvider.GetDefaultFont(), screen));
            Mock.Get(frenchHidLayout)
               .Setup(x => x.Keys)
               .Returns(keys);

            var englishHidLayout = Mock.Of<ILayout>();
            Mock.Get(englishHidLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(new OsLayoutId("english"), false, true, null, false, true, true));
            Mock.Get(englishHidLayout)
                .Setup(x => x.LayoutImageInfo)
                .Returns(new LayoutImageInfo(HexColor.White, FontProvider.GetDefaultFont(), screen));
            Mock.Get(englishHidLayout)
               .Setup(x => x.Keys)
               .Returns(keys);

            var customEnglishLayout = Mock.Of<ILayout>();

            Mock.Get(customEnglishLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(new OsLayoutId("english"), false, false, null, false, true, true));
            Mock.Get(customEnglishLayout)
                .Setup(x => x.LayoutImageInfo)
                .Returns(new LayoutImageInfo(HexColor.White, FontProvider.GetDefaultFont(), screen));
            Mock.Get(customEnglishLayout)
               .Setup(x => x.Keys)
               .Returns(keys);

            Mock.Get(_augmentedLayoutImageProvider)
                .Setup(x => x.AugmentedLayoutImageExists(It.IsAny<ILayout>()))
                .Returns(true);

            Mock.Get(_genService)
                .Setup(x => x.RenderLayoutImage(It.IsAny<ImageRequest>()))
                .Returns(new byte[] { });

            Mock.Get(_library)
                .Setup(x => x.UpdateLayoutAsync(It.IsAny<ILayout>()))
                .ReturnsAsync(() => _layout)
                .Callback(() => 
                {
                    libraryUpdateCalled = true;
                    libraryUpdateCountCalled += 1;
                });

            Mock.Get(_library)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>() { frenchHidLayout, englishHidLayout, customEnglishLayout });

            Mock.Get(_synchronizer)
                .Setup(x => x.SynchronizeAsync())
                .Returns(Task.Delay(1))
                .Callback(() => synchronizeCalled = true);

            await _facade.RefreshAugmentedLayoutAsync();

            libraryUpdateCalled.Should().BeTrue();
            synchronizeCalled.Should().BeTrue();
            libraryUpdateCountCalled.Should().Be(2);
        }
    }
}
