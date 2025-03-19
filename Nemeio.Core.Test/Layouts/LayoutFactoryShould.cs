using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Exceptions;
using Nemeio.Core.Images;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts;
using Nemeio.Core.Layouts.Export;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Layouts.Name;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services.Layouts;
using NUnit.Framework;

namespace Nemeio.Core.Test.Layouts
{
    [TestFixture]
    public class LayoutFactoryShould
    {
        private ILayoutLibrary _library;
        private ILayoutImageGenerator _genService;
        private ILayoutNameTransformer _layoutNameTransformer;
        private LayoutFactory _factory;
        private IScreenFactory _screenFactory;

        [SetUp]
        public void SetUp()
        {
            _library = Mock.Of<ILayoutLibrary>();
            _genService = Mock.Of<ILayoutImageGenerator>();
            _layoutNameTransformer = Mock.Of<ILayoutNameTransformer>();
            _screenFactory = Mock.Of<IScreenFactory>();
            _factory = new LayoutFactory(_library, _genService, _layoutNameTransformer, _screenFactory);
        }

        [Test]
        public void LayoutFactory_CreateHid_WhenIdIsNull_Throws()
        {
            var screen = Mock.Of<IScreen>();

            Assert.Throws<ArgumentNullException>(() => _factory.CreateHid(null, screen, "myFakeName"));
        }

        [Test]
        public void LayoutFactory_CreateHid_WhenNameIsNullOrEmpty_Throws([Values(null, "")] string name)
        {
            var screen = Mock.Of<IScreen>();
            var fakeOsLayoutId = new OsLayoutId("frenchLayout");

            Assert.Throws<ArgumentNullException>(() => _factory.CreateHid(fakeOsLayoutId, screen, name));
        }

        [Test]
        public void LayoutFactory_CreateHid_Ok([Values] ScreenType screenType)
        {
            Mock.Get(_genService)
                .Setup(x => x.CreateLayoutKeys(It.IsAny<IScreen>(), It.IsAny<OsLayoutId>()))
                .Returns(new List<Key>());
            Mock.Get(_genService)
                .Setup(x => x.RenderLayoutImage(It.IsAny<ImageRequest>()))
                .Returns(new byte[] { });

            var osLayoutId = new OsLayoutId("frenchLayout");
            var layoutName = "french";

            var screen = Mock.Of<IScreen>();
            var layout = _factory.CreateHid(osLayoutId, screen, layoutName);

            layout.Should().NotBeNull();
            layout.LayoutInfo.OsLayoutId.Should().Be(osLayoutId);
            layout.LayoutInfo.Factory.Should().BeFalse();
            layout.LayoutInfo.Hid.Should().BeTrue();
            layout.LayoutInfo.LinkApplicationPaths.Should().BeEmpty();
            layout.LayoutInfo.LinkApplicationEnable.Should().BeFalse();
            layout.LayoutInfo.IsTemplate.Should().BeTrue();
            layout.LayoutInfo.AugmentedHidEnable.Should().BeTrue();
            layout.LayoutImageInfo.ImageType.Should().Be(LayoutImageType.Classic);
            layout.LayoutImageInfo.Font.Should().BeEquivalentTo(FontProvider.GetDefaultFont());
            layout.LayoutImageInfo.Color.IsBlack().Should().BeTrue();
            layout.Title.Should().Be(layoutName);
        }

        [Test]
        public void LayoutFactory_Duplicate_WhenLayoutIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _factory.Duplicate(null, "duplicatedLayoutTitle"));
        }

        [Test]
        public void LayoutFactory_Duplicate_WhenLayoutIsAnHid_Throws()
        {
            var layout = Mock.Of<ILayout>();
            Mock.Get(layout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(new OsLayoutId("frenchLayout"), false, true));

            Assert.Throws<ForbiddenActionException>(() => _factory.Duplicate(layout, "duplicatedLayoutTitle"));
        }

        [Test]
        public void LayoutFactory_Duplicate_Ok()
        {
            var calculateImageHashCalled = false;

            var layout = Mock.Of<ILayout>();
            Mock.Get(layout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(new OsLayoutId("frenchLayout"), false, false));
            Mock.Get(layout)
                .Setup(x => x.CalculateImageHash())
                .Callback(() => calculateImageHashCalled = true);
            Mock.Get(layout)
                .Setup(x => x.CreateBackup())
                .Returns(layout);

            var result = _factory.Duplicate(layout, "duplicatedLayoutTitle");

            result.Should().NotBeNull();
            result.Enable.Should().BeFalse();
            calculateImageHashCalled.Should().BeTrue();
        }

        [Test]
        public void LayoutFactory_CreateFromExport_WhenParameterIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _factory.CreateFromExport(null));
        }

        [Test]
        public void LayoutFactory_CreateFromExport_Ok()
        {
            var export = new LayoutExport()
            {
                LinkApplicationPaths = new List<string>() { "this/is/my/fake/app/path.exe" },
                LinkApplicationEnable = true,
                IsDarkMode = false,
                Font = new Font("MyFontName", Nemeio.Models.Fonts.FontSize.Large, false, true),
                ImageType = LayoutImageType.Hide,
                Keys = new List<Key>() 
                { 
                    new Key() 
                    { 
                        Index = 0,
                        Disposition = Enums.KeyDisposition.Double,
                        Edited = true,
                        Actions = new List<KeyAction>()
                        {
                            new KeyAction()
                            {
                                Display = "A",
                                Modifier = Enums.KeyboardModifier.None,
                                Subactions = new List<KeySubAction>()
                            }
                        }
                    } 
                },
                Title = "myExportTitle",
                AssociatedLayoutId = "AAA2CD6B-C574-4450-BC9C-F5520EAFF5EA"
            };

            var result = _factory.CreateFromExport(export);

            result.Should().NotBeNull();
            result.LayoutInfo.AugmentedHidEnable.Should().BeFalse();
            result.LayoutInfo.LinkApplicationPaths.Should().BeEquivalentTo(export.LinkApplicationPaths);
            result.LayoutInfo.LinkApplicationEnable.Should().Be(export.LinkApplicationEnable);
            result.LayoutImageInfo.Color.IsBlack().Should().Be(export.IsDarkMode);
            result.LayoutImageInfo.Font.Should().Be(export.Font);
            result.LayoutImageInfo.ImageType.Should().Be(export.ImageType);
            result.Keys.Should().BeEquivalentTo(export.Keys);
            result.Title.Should().Be(export.Title);
            result.AssociatedLayoutId.Should().Be(new LayoutId(export.AssociatedLayoutId));
            result.IsDefault.Should().BeFalse();
            result.Enable.Should().BeFalse();
            result.LayoutInfo.IsTemplate.Should().BeFalse();
            result.LayoutInfo.Factory.Should().BeFalse();
            result.LayoutInfo.Hid.Should().BeFalse();
        }

        [Test]
        public void LayoutFactory_CreateHids_WithEmptyList_MustReturnEmptyList_Ok([Values] ScreenType screenType)
        {
            var screen = Mock.Of<IScreen>();
            var osLayouts = Enumerable.Empty<OsLayoutId>();

            var result = _factory.CreateHids(osLayouts, screen);

            result.Should().BeEmpty();
        }

        [Test]
        public void LayoutFactory_CreateHids_Ok([Values] ScreenType screenType)
        {
            var frenchOsLayout = new OsLayoutId("french");
            var englishOsLayout = new OsLayoutId("english");
            var osLayouts = new List<OsLayoutId>() { frenchOsLayout, englishOsLayout };

            Mock.Get(_layoutNameTransformer)
                .Setup(x => x.TransformNameIfNeeded(It.IsAny<IList<ILayout>>(), It.IsAny<OsLayoutId>()))
                .Returns("myFakeName");

            var screen = Mock.Of<IScreen>();
            var result = _factory.CreateHids(osLayouts, screen);

            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Count().Should().Be(osLayouts.Count);
        }
    }
}
