using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Api.Dto;
using Nemeio.Api.Dto.In.Layout;
using Nemeio.Api.Exceptions;
using Nemeio.Api.PatchApplier;
using Nemeio.Core;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Errors;
using Nemeio.Core.Images;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts;
using Nemeio.Core.Layouts.Color;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Layouts.LinkedApplications;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Test.Fakes;
using Nemeio.Models.Fonts;
using NUnit.Framework;

namespace Nemeio.Api.Test.Patchers
{
    [TestFixture]
    public class LayoutPatchApplierShould
    {
        private const string FakeName = "fake.exe";
        private string FakeApplicationPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), FakeName);
        private FileWatcher _fakePathWatcher;

        private ILayout _inputLayout;
        private ILayout _anotherLayout;
        private ILayout _customLayout;
        private LayoutPatchApplier _layoutPatchApplier;
        private Font _layoutFont;
        private IFontProvider _fontProvider;
        private IErrorManager _mockErrorManager;
        private IApplicationLayoutManager _mockApplicationLayoutManager;

        private bool _renderLayoutImageCalled = false;

        [OneTimeSetUp]
        public void GlobalSetUp()
        {
            _fakePathWatcher = FileHelpers.WatchMe(FakeApplicationPath);
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            if (_fakePathWatcher != null)
            {
                _fakePathWatcher.Dispose();
                _fakePathWatcher = null;
            }
        }

        [SetUp]
        public void SetUp()
        {
            _renderLayoutImageCalled = false;
            _layoutFont = FontProvider.GetDefaultFont();

            var screen = Mock.Of<IScreen>();

            _inputLayout = new Nemeio.Core.Services.Layouts.Layout(
                new LayoutInfo(
                    new OsLayoutId("fakeInputLayoutId"),
                    true,
                    true,
                    new List<string>(),
                    true
                ),
                new LayoutImageInfo(HexColor.Black, _layoutFont, screen),
                new byte[1] { 0xFF },
                456,
                5,
                "input Layout Title",
                "input Layout Subtitle",
                new DateTime(2018, 12, 03),
                new DateTime(2018, 12, 03),
                new List<Key>(),
                null,
                null,
                false
            );

            _customLayout = new Nemeio.Core.Services.Layouts.Layout(
                new LayoutInfo(
                    new OsLayoutId("fakeSeconbdLayoutId"),
                    true,
                    false,
                    new List<string>() { FakeApplicationPath },
                    true
                ),
                new LayoutImageInfo(HexColor.Black, _layoutFont, screen),
                new byte[0],
                456,
                5,
                "custom Layout Title",
                "custom Layout Subtitle",
                new DateTime(2018, 12, 03),
                new DateTime(2018, 12, 03),
                new List<Key>(),
                null,
                null,
                false
            );

            _anotherLayout = new Nemeio.Core.Services.Layouts.Layout(
                new LayoutInfo(
                    new OsLayoutId("fakeSecondLayoutId"),
                    true,
                    true,
                    new List<string>() { FakeApplicationPath },
                    true
                ),
                new LayoutImageInfo(HexColor.Black, _layoutFont, screen),
                new byte[0],
                456,
                5,
                "second Layout Title",
                "second Layout Subtitle",
                new DateTime(2018, 12, 03),
                new DateTime(2018, 12, 03),
                new List<Key>(),
                null,
                null,
                false
            );

            var mockLayoutGenService = Mock.Of<ILayoutImageGenerator>();
            Mock.Get(mockLayoutGenService)
                .Setup(x => x.RenderLayoutImage(It.IsAny<ImageRequest>()))
                .Callback(() => _renderLayoutImageCalled = true)
                .Returns(() =>
                {
                    return new byte[1] { 0x00 };
                });

            _mockApplicationLayoutManager = Mock.Of<IApplicationLayoutManager>();

            var mockLayoutLibrary = Mock.Of<ILayoutLibrary>();
            Mock.Get(mockLayoutLibrary)
                .Setup(mock => mock.Layouts)
                .Returns(new List<ILayout>()
                {
                    _inputLayout,
                    _customLayout,
                    _anotherLayout
                });

            var loggerFactory = new LoggerFactory();

            _mockErrorManager = Mock.Of<IErrorManager>();

            Mock.Get(_mockErrorManager)
                .Setup(x => x.GetFullErrorMessage(It.IsAny<ErrorCode>(), It.IsAny<Exception>()))
                .Returns(string.Empty);

            _fontProvider = new FontProvider(loggerFactory, _mockErrorManager);
            _layoutPatchApplier = new LayoutPatchApplier(loggerFactory, mockLayoutGenService, mockLayoutLibrary, _mockApplicationLayoutManager, _fontProvider);
        }

        [Test]
        public void LayoutPatchApplier_Patch_WithNullInputValue_ThrowsGivenException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _layoutPatchApplier.Patch(null, _inputLayout);
            });
        }

        [Test]
        public void LayoutPatchApplier_Patch_WithNullCurrentValue_ThrowsGivenException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _layoutPatchApplier.Patch(new PutLayoutApiInDto(), null);
            });
        }

        [Test]
        public void LayoutPatchApplier_Patch_UpdateLayoutIndex_WorksOk()
        {
            var newIndex = 300;
            var oldIndex = _inputLayout.Index;

            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.Index = newIndex;

            var updatedLayout = _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);

            updatedLayout.Index.Should().NotBe(oldIndex);
            updatedLayout.Index.Should().Be(newIndex);
        }

        [Test]
        public void LayoutPatchApplier_Patch_UpdateLayoutTitle_WorksOk()
        {
            var newTitle = "my_new_fake_title";
            var oldTitle = _inputLayout.Title;

            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.Title = newTitle;

            var updatedLayout = _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);

            updatedLayout.Title.Should().NotBe(oldTitle);
            updatedLayout.Title.Should().Be(newTitle);
        }

        [Test]
        public void LayoutPatchApplier_Patch_UpdateLayoutTitle_WithEmptyTitle_ThrowsPatchFailedException()
        {
            var newTitle = string.Empty;

            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.Title = newTitle;

            var exception = Assert.Throws<PatchFailedException>(() =>
            {
                _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);
            });

            exception.ErrorCode.Should().Be((int)LayoutPatchError.EmptyLayoutName);
        }

        [Test]
        public void LayoutPatchApplier_Patch_UpdateLayoutTitle_WithTitleAlreadyUsed_ThrowsPatchFailedException()
        {
            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.Title = _anotherLayout.Title;

            var exception = Assert.Throws<PatchFailedException>(() =>
            {
                _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);
            });

            exception.ErrorCode.Should().Be((int)LayoutPatchError.LayoutNameAlreadyUsed);
        }

        [Test]
        public void LayoutPatchApplier_Patch_UpdateLayoutCategoryId_WorksOk()
        {
            var newCategoryId = 148;
            var oldCategoryId = _inputLayout.CategoryId;

            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.CategoryId = newCategoryId;

            var updatedLayout = _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);

            updatedLayout.CategoryId.Should().NotBe(oldCategoryId);
            updatedLayout.CategoryId.Should().Be(newCategoryId);
        }

        [Test]
        public void LayoutPatchApplier_Patch_UpdateLayoutEnable_WorksOk()
        {
            var oldEnable = _inputLayout.Enable;
            var newEnable = _inputLayout.LayoutInfo.Hid ? oldEnable : !oldEnable;

            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.Enable = newEnable;

            var updatedLayout = _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);

            updatedLayout.Enable.Should().Be(newEnable);
        }

        [Test]
        public void LayoutPatchApplier_Patch_UpdateLayoutLinkAppPath_WorksOk()
        {
            var fakeFolderPath = Path.Combine(Path.GetTempPath(), @"AnotherFake");
            var fakeApplicationPath = Path.Combine(fakeFolderPath, FakeName).ToLower();
            Assert.False(Directory.Exists(fakeFolderPath));
            using (var watchMe = FileHelpers.WatchMe(fakeApplicationPath))
            {
                var beforeInputValue = _inputLayout.LayoutInfo.LinkApplicationPaths;

                var tempPath = new List<string>() { fakeApplicationPath };

                var putLayoutInDto = new PutLayoutApiInDto();
                putLayoutInDto.LinkApplicationPath = tempPath;

                var updatedLayout = _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);

                updatedLayout.LayoutInfo.LinkApplicationPaths.Should().NotBeEquivalentTo(beforeInputValue);
                updatedLayout.LayoutInfo.LinkApplicationPaths.Should().BeEquivalentTo(tempPath);
            }
            Assert.False(File.Exists(fakeApplicationPath));
            Assert.False(Directory.Exists(fakeFolderPath));
        }

        [Test]
        public void LayoutPatchApplier_Patch_WhenAddSameSoftware_MultipleTimes_WorksOk()
        {
            var outlookFakeFolderPath = Path.Combine(Path.GetTempPath(), @"Outlook");
            var outlookFakeApplicationPath = Path.Combine(outlookFakeFolderPath, @"outlook.exe").ToLower();

            Assert.False(Directory.Exists(outlookFakeFolderPath));

            var paths = new List<string>() { outlookFakeApplicationPath, outlookFakeApplicationPath };
            using (var watch1 = FileHelpers.WatchMe(outlookFakeApplicationPath))
            {
                var putLayoutInDto = new PutLayoutApiInDto();
                putLayoutInDto.LinkApplicationPath = paths;

                var updatedLayout = _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);

                updatedLayout.LayoutInfo.LinkApplicationPaths.Count().Should().Be(1);
                updatedLayout.LayoutInfo.LinkApplicationPaths.Should().BeEquivalentTo(new List<string>() { outlookFakeApplicationPath });
            }

            Assert.False(Directory.Exists(outlookFakeFolderPath));
        }

        [Test]
        public void LayoutPatchApplier_Patch_UpdateLayoutLinkAppPathWithMultipleValues_WorksOk()
        {
            var outlookFakeFolderPath = Path.Combine(Path.GetTempPath(), @"Outlook");
            var outlookFakeApplicationPath = Path.Combine(outlookFakeFolderPath, @"outlook.exe").ToLower();
            var excelFakeFolderPath = Path.Combine(Path.GetTempPath(), @"Outlook");
            var excelFakeApplicationPath = Path.Combine(excelFakeFolderPath, @"excel.exe").ToLower();
            Assert.False(Directory.Exists(outlookFakeFolderPath));
            Assert.False(Directory.Exists(excelFakeFolderPath));

            var beforeInputValue = _inputLayout.LayoutInfo.LinkApplicationPaths;
            var paths = new List<string>() { outlookFakeApplicationPath, excelFakeApplicationPath };
            using (var watch1 = FileHelpers.WatchMe(outlookFakeApplicationPath))
            using (var watch2 = FileHelpers.WatchMe(excelFakeApplicationPath))
            {
                var putLayoutInDto = new PutLayoutApiInDto();
                putLayoutInDto.LinkApplicationPath = paths;

                var updatedLayout = _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);

                updatedLayout.LayoutInfo.LinkApplicationPaths.Should().NotBeEquivalentTo(beforeInputValue);
                updatedLayout.LayoutInfo.LinkApplicationPaths.Should().BeEquivalentTo(paths);
            }
            Assert.False(Directory.Exists(outlookFakeFolderPath));
            Assert.False(Directory.Exists(excelFakeFolderPath));
        }

        [Test]
        public void LayoutPatchApplier_Patch_UpdateLayoutLinkAppPathWithInvalidValue_ThrowsPatchFailedException()
        {
            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.LinkApplicationPath = new List<string>()
            {
                @"Fake.exe",
                @"C:\Program Files\DoesNotExist\excel.exe",
            }; ;

            var exception = Assert.Throws<PatchFailedException>(() =>
            {
                _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);
            });

            exception.ErrorCode.Should().Be((int)LayoutPatchError.InvalidPath);
        }

        [Test]
        public void LayoutPatchApplier_Patch_UpdateLayoutLinkAppPathWithInvalidExecutableExtension_ThrowsPatchFailedException()
        {
            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.LinkApplicationPath = new List<string>()
            {
                @"C:\Program Files\DoesNotExist\excel.py",
            }; ;

            var exception = Assert.Throws<PatchFailedException>(() =>
            {
                _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);
            });

            exception.ErrorCode.Should().Be((int)LayoutPatchError.NotAnExecutable);
        }

        [Test]
        public void LayoutPatchApplier_Patch_UpdateLayoutLinkAppPathWithAlreadyTakenApplication_ThrowsPatchFailedException()
        {
            Assert.True(File.Exists(FakeApplicationPath));

            var loggerFactory = new LoggerFactory();
            var mockLayoutGenService = Mock.Of<ILayoutImageGenerator>();
            var mockLayoutLibrary = Mock.Of<ILayoutLibrary>();

            var mockApplicationLayoutManager = Mock.Of<IApplicationLayoutManager>();
            Mock.Get(mockApplicationLayoutManager)
                .Setup(x => x.GetLayoutByLinkedApplicationPath(It.IsAny<string>()))
                .Returns(_customLayout);

            var layoutPatchApplier = new LayoutPatchApplier(loggerFactory, mockLayoutGenService, mockLayoutLibrary, mockApplicationLayoutManager, _fontProvider);

            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.LinkApplicationPath = new List<string>() { FakeApplicationPath };

            Assert.Throws<PatchFailedException>(() =>
            {
                layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);
            });
        }

        [Test]
        public void LayoutPatchApplier_Patch_UpdateLayoutLinkAppEnable_WorksOk()
        {
            var newInputValue = _inputLayout.LayoutInfo.Hid;
            var beforeInputValue = _inputLayout.LayoutInfo.LinkApplicationEnable;

            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.LinkApplicationEnable = true;

            var updatedLayout = _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);

            updatedLayout.Enable.Should().Be(newInputValue);
        }

        [Test]
        public void LayoutPatchApplier_Patch_UpdateLayoutAssociatedLayoutId_WorksOk()
        {
            var newAssociatedLayoutId = FakeLayoutDbRepository.FakeLayoutId1;
            var oldAssociatedLayoutId = _customLayout.AssociatedLayoutId;

            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.AssociatedId = new Optional<string>() { Value = newAssociatedLayoutId };

            var updatedLayout = _layoutPatchApplier.Patch(putLayoutInDto, _customLayout);

            updatedLayout.AssociatedLayoutId.ToString().Should().NotBe(oldAssociatedLayoutId);
            updatedLayout.AssociatedLayoutId.ToString().Should().Be(newAssociatedLayoutId);
        }

        [Test]
        public void LayoutPatchApplier_Patch_UpdateLayoutLinkAppPathWithValidValues_WorksOk()
        {
            var fakeFolderPath = Path.Combine(Path.GetTempPath(), @"AnotherFake");
            var fakeApplicationPath = Path.Combine(fakeFolderPath, FakeName).ToLower();
            Assert.False(Directory.Exists(fakeFolderPath));
            using (var watchMe = FileHelpers.WatchMe(fakeApplicationPath))
            {
                var beforeInputValue = _inputLayout.LayoutInfo.LinkApplicationPaths;

                var tempPath = new List<string>() { FakeName, fakeApplicationPath };

                var putLayoutInDto = new PutLayoutApiInDto();
                putLayoutInDto.LinkApplicationPath = tempPath;

                var updatedLayout = _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);

                updatedLayout.LayoutInfo.LinkApplicationPaths.Should().NotBeEquivalentTo(beforeInputValue);
                updatedLayout.LayoutInfo.LinkApplicationPaths.Should().BeEquivalentTo(tempPath);
            }
            Assert.False(File.Exists(fakeApplicationPath));
            Assert.False(Directory.Exists(fakeFolderPath));
        }

        [Test]
        public void LayoutPatchApplier_Patch_UpdateLayoutTitle_WithSameTitle_NotThrowsPatchFailedException()
        {
            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.Title = _inputLayout.Title;

            Assert.DoesNotThrow(() => _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void LayoutPatchApplier_Patch_UpdateLayoutBackgroundColorWithTitle_WorksOk(bool isDarkMode)
        {
            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.IsDarkMode = isDarkMode;
            putLayoutInDto.Title = _inputLayout.Title;

            Assert.DoesNotThrow(() => _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout));
        }

        [Test]
        public void LayoutPatchApplier_Patch_UpdateLayoutAssociatedLayoutIdOnHid_WorksOk()
        {
            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.AssociatedId = new Optional<string>() { Value = FakeLayoutDbRepository.FakeLayoutId4 };

            var updatedLayout = _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);

            updatedLayout.AssociatedLayoutId.Should().BeNull();
        }

        [Test]
        public void LayoutPatchApplier_Patch_UpdateFont_WorksOk()
        {
            var newFontName = NemeioConstants.Cairo;
            const FontSize newFontSize = FontSize.Small;
            const bool newFontBoldStatus = true;
            const bool newFontItalicStatus = false;

            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.Font = new PutFontInDto()
            {
                Name = newFontName,
                Size = newFontSize,
                Bold = newFontBoldStatus,
                Italic = newFontItalicStatus
            };

            var updatedLayout = _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);

            updatedLayout.LayoutImageInfo.Font.Should().NotBeNull();
            updatedLayout.LayoutImageInfo.Font.Name.Should().Be(newFontName);
            updatedLayout.LayoutImageInfo.Font.Size.Should().Be(newFontSize);
            updatedLayout.LayoutImageInfo.Font.Bold.Should().Be(newFontBoldStatus);
            updatedLayout.LayoutImageInfo.Font.Italic.Should().Be(newFontItalicStatus);
        }

        [Test]
        public void LayoutPatchApplier_Patch_UpdateFont_WhenIsNull_WorksOk()
        {
            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.AssociatedId = new Optional<string>() { Value = FakeLayoutDbRepository.FakeLayoutId4 };

            var updatedLayout = _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);

            updatedLayout.LayoutImageInfo.Font.Should().NotBeNull();
            updatedLayout.LayoutImageInfo.Font.Name.Should().Be(_layoutFont.Name);
            updatedLayout.LayoutImageInfo.Font.Size.Should().Be(_layoutFont.Size);
            updatedLayout.LayoutImageInfo.Font.Bold.Should().Be(_layoutFont.Bold);
            updatedLayout.LayoutImageInfo.Font.Italic.Should().Be(_layoutFont.Italic);
        }

        [Test]
        public void LayoutPatchApplier_Patch_RecreateLayoutImage_WhenDarkModeChanged()
        {
            //  Not change dark mode
            var putLayoutInDto = new PutLayoutApiInDto();

            _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);
            _renderLayoutImageCalled.Should().BeFalse();

            //  Change dark mode
            _renderLayoutImageCalled = false;

            putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.IsDarkMode = !_inputLayout.LayoutImageInfo.Color.IsBlack();

            _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);
            _renderLayoutImageCalled.Should().BeTrue();
        }

        [Test]
        public void LayoutPatchApplier_Patch_RecreateLayoutImage_WhenFontChanged()
        {
            //  Not change font
            var putLayoutInDto = new PutLayoutApiInDto();

            _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);
            _renderLayoutImageCalled.Should().BeFalse();

            //  Change font
            _renderLayoutImageCalled = false;

            putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.Font = new Optional<PutFontInDto>()
            {
                Value = new PutFontInDto()
                {
                    Name = NemeioConstants.Cairo,
                    Size = FontSize.Small,
                    Bold = true,
                    Italic = false
                }
            };

            _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout);
            _renderLayoutImageCalled.Should().BeTrue();
        }

        [Test]
        public void LayoutPatchApplier_Patch_WhenFontIsNull_ThrowPatchFailedException()
        {
            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.Font = null;

            var exception = Assert.Throws<PatchFailedException>(() => _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout));

            exception.ErrorCode.Should().Be((int)LayoutPatchError.InvalidLayoutFont);
        }

        [Test]
        public void LayoutPatchApplier_Patch_WhenFontIsUnknown_ThrowPatchFailedException()
        {
            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.Font = new Optional<PutFontInDto>()
            {
                Value = new PutFontInDto()
                {
                    Name = "this_font_is_unknown.ttf",
                    Size = FontSize.Large,
                    Bold = true,
                    Italic = false
                }
            };

            var exception = Assert.Throws<PatchFailedException>(() => _layoutPatchApplier.Patch(putLayoutInDto, _inputLayout));

            exception.ErrorCode.Should().Be((int)LayoutPatchError.InvalidLayoutFont);
        }

        [TestCase(null)]
        [TestCase("")]
        public void LayoutPatchApplier_Patch_WhenAssociatedIdIsNull_ThrowPatchFailedException(string associatedIdValue)
        {
            var putLayoutInDto = new PutLayoutApiInDto();
            putLayoutInDto.AssociatedId = new Optional<string>()
            {
                Value = associatedIdValue
            };

            var exception = Assert.Throws<PatchFailedException>(() => _layoutPatchApplier.Patch(putLayoutInDto, _customLayout));

            exception.ErrorCode.Should().Be((int)LayoutPatchError.InvalidLayoutAssociatedId);
        }
    }
}
