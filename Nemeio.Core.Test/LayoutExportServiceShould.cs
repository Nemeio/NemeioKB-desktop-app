using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.Core.Errors;
using Nemeio.Core.Exceptions;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Color;
using Nemeio.Core.Layouts.Export;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services.Layouts;
using NUnit.Framework;

namespace Nemeio.Core.Test
{
    public class LayoutExportServiceShould
    {
        private LayoutExportService _layoutExportService;
        private ILayoutDbRepository _layoutDbRepository;

        [SetUp]
        public void SetUp()
        {
            _layoutDbRepository = Mock.Of<ILayoutDbRepository>();
            _layoutExportService = new LayoutExportService(new LoggerFactory(), _layoutDbRepository);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void LayoutExportService_Export_WithInvalidStringIdParameter_ThrowArgumentNullException(string id)
        {
            Assert.Throws<ArgumentNullException>(() => _layoutExportService.Export(id));
        }

        [TestCase("a")]
        [TestCase("this_is_my_id")]
        public void LayoutExportService_Export_WithInvalidGuidIdParameter_ThrowCoreException(string id)
        {
            var exception = Assert.Throws<CoreException>(() => _layoutExportService.Export(id));

            exception.ErrorCode.Should().Be(ErrorCode.CoreExportLayoutInvalidId);
        }

        [Test]
        public void LayoutExportService_Export_WithNullIdParameter_ThrowArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _layoutExportService.Export(layoutId: null, ExportFileType.API));
        }

        [Test]
        public void LayoutExportService_Export_WhenLayoutIdIsUnknown_ThrowCoreException()
        {
            const string layoutId = "097CC4E4-DBEF-4190-9AEB-47B985C33A9D";

            var myLayoutId = new LayoutId(layoutId);

            Mock.Get(_layoutDbRepository)
                .Setup(x => x.FindLayoutById(It.IsAny<LayoutId>()))
                .Throws(new InvalidOperationException(""));

            var exception = Assert.Throws<CoreException>(() => _layoutExportService.Export(myLayoutId));

            exception.ErrorCode.Should().Be(ErrorCode.CoreExportLayoutIsNotFound);
        }

        [Test]
        public void LayoutExportService_Export_WhenLayoutIdHid_ThrowCoreException()
        {
            const string layoutId = "097CC4E4-DBEF-4190-9AEB-47B985C33A9D";

            var screen = Mock.Of<IScreen>();
            var myLayoutId = new LayoutId(layoutId);
            var myLayout = new Layout(
                new LayoutInfo(
                    new OsLayoutId(""),
                    isFactory: false,
                    isHid: true
                ),
                new LayoutImageInfo(
                    HexColor.Black,
                    font: FontProvider.GetDefaultFont(),
                    screen
                ),
                new byte[0],
                categoryId: 0,
                index: 0,
                title: "My Title",
                subtitle: "My Subitle",
                creation: DateTime.Now,
                update: DateTime.Now,
                keys: new List<Key>(),
                myLayoutId
            );

            Mock.Get(_layoutDbRepository)
                .Setup(x => x.FindLayoutById(It.IsAny<LayoutId>()))
                .Returns(myLayout);

            var exception = Assert.Throws<CoreException>(() => _layoutExportService.Export(myLayoutId));

            exception.ErrorCode.Should().Be(ErrorCode.CoreExportLayoutHidForbidden);
        }

        [Test]
        public void LayoutExportService_Export_Ok()
        {
            const string layoutId = "097CC4E4-DBEF-4190-9AEB-47B985C33A9D";
            const string layoutTitle = "My Title";
            const string layoutSubtitle = "My Subtitle";
            const LayoutImageType layoutImageType = LayoutImageType.Hide;
            const bool darkMode = true;
            const bool linkAppEnabled = true;
            const string linkApp = @"C:\Users\myUser\programs\myGreatProgram.exe";
            const string associatedLayoutId = "091D205C-1174-4DA7-BBA7-669FE6FADD5F";

            var keys = new List<Key>()
            {
                new Key()
                {
                    Index = 0,
                    Edited = false,
                    Actions = new List<KeyAction>()
                    {
                        new KeyAction()
                        {
                            Display = "a",
                            Subactions = new List<KeySubAction>()
                            {
                                new KeySubAction("a", Enums.KeyActionType.Unicode)
                            }
                        }
                    }
                },
                new Key()
                {
                    Index = 1,
                    Edited = true,
                    Actions = new List<KeyAction>()
                    {
                        new KeyAction()
                        {
                            Display = "b",
                            Subactions = new List<KeySubAction>()
                            {
                                new KeySubAction("b", Enums.KeyActionType.Unicode)
                            }
                        }
                    }
                }
            };

            var screen = Mock.Of<IScreen>();
            var numberOfEditedKeys = keys.Count(x => x.Edited);

            var myLayoutId = new LayoutId(layoutId);
            var myLayout = new Layout(
                new LayoutInfo(
                    new OsLayoutId(""),
                    isFactory: false,
                    isHid: false,
                    isTemplate: false,
                    linkEnable: linkAppEnabled,
                    linkPath: new List<string>() { linkApp }
                ),
                new LayoutImageInfo(
                    HexColor.Black,
                    font: FontProvider.GetDefaultFont(),
                    screen: screen,
                    imageType: layoutImageType
                ),
                new byte[0],
                categoryId: 0,
                index: 0,
                title: layoutTitle,
                subtitle: layoutSubtitle,
                creation: DateTime.Now,
                update: DateTime.Now,
                keys: keys,
                myLayoutId,
                associatedLayoutId: new LayoutId(associatedLayoutId)
            );

            Mock.Get(_layoutDbRepository)
                .Setup(x => x.FindLayoutById(It.IsAny<LayoutId>()))
                .Returns(myLayout);

            var export = _layoutExportService.Export(myLayoutId);

            export.Filename.Should().Be($"{layoutTitle}.{LayoutExportService.ExportExtension}");
            export.Title.Should().Be(layoutTitle);
            export.Keys.Count().Should().Be(numberOfEditedKeys);
            export.ImageType.Should().Be(layoutImageType);
            export.IsDarkMode.Should().Be(darkMode);
            export.Version.Should().NotBeNullOrWhiteSpace();
            export.Version.Should().Be($"{export.MajorVersion}.{export.MinorVersion}");
            export.LinkApplicationEnable.Should().Be(myLayout.LayoutInfo.LinkApplicationEnable);
            export.LinkApplicationPaths.Count().Should().Be(1);
            export.LinkApplicationPaths.ElementAt(0).Should().Be(linkApp.ToLower());
            export.AssociatedLayoutId.Should().Be(new LayoutId(associatedLayoutId));
            export.Font.Should().NotBeNull();
        }
    }
}
