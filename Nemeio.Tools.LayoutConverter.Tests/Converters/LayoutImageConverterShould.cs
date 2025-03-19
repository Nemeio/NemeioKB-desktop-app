using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Moq;
using Nemeio.Tools.LayoutConverter.Converters;
using Nemeio.Tools.LayoutConverter.Exceptions;
using Nemeio.Tools.LayoutConverter.Factories;
using Nemeio.Tools.LayoutConverter.Models;
using Nemeio.Tools.LayoutConverter.Providers;
using NUnit.Framework;

namespace Nemeio.Tools.LayoutConverter.Tests.Converters
{
    public class LayoutImageConverterShould
    {
        private IAugmentedImageFileProvider _augmentedImageFileProvider;
        private IPathProvider _pathProvider;

        [SetUp]
        public void SetUp()
        {
            _augmentedImageFileProvider = Mock.Of<IAugmentedImageFileProvider>();
            _pathProvider = Mock.Of<IPathProvider>();

            Mock.Get(_pathProvider)
                .Setup(x => x.GetNemeioApplicationPath())
                .Returns(() => 
                {
                    return Directory.GetCurrentDirectory();
                });
        }

        [Test]
        public void LayoutImageConverter_CreateWallpaper_FolderDoesNotExists_ThrowToolException()
        {
            Mock.Get(_augmentedImageFileProvider)
                .Setup(x => x.GetFilesFrom(It.IsAny<string>()))
                .Throws(new ToolException(ToolErrorCode.DirectoryNotFound));

            var imageType = new ImageType("Fake", new List<string>() { Constantes.NoneModifier });
            var informations = new ImageConversionInformation("95", @"C:\this\is\a\fake\path\file.png", "1bpp", imageType);
            var layoutImageConverter = new LayoutImageConverter(informations, _augmentedImageFileProvider, _pathProvider);

            var exception = Assert.Throws<InvalidInputException>(() => 
            {
                layoutImageConverter.CreateWallpaper();
            });

            exception.ErrorCode.Should().Be(ToolErrorCode.InvalidInput);
            exception.Type.Should().Be(InputType.FolderPath);
        }

        [TestCase("4bpp")]
        [TestCase("16bpp")]
        [TestCase("this_is_an_invalid_bpp")]
        public void ImageConversionInformation_Constructor_ImageFormatIsInvalid_ThrowsInvalidInputException(string format)
        {
            var imageType = new ImageType("Fake", new List<string>() { Constantes.NoneModifier });
            var mockAugmentedImageFileProvider = Mock.Of<IAugmentedImageFileProvider>();

            var informations = new ImageConversionInformation("95", @"C:\this\is\a\fake\path\file.png", format, imageType);
            var exception = Assert.Throws<InvalidInputException>(() => new LayoutImageConverter(informations, mockAugmentedImageFileProvider, _pathProvider));

            exception.ErrorCode.Should().Be(ToolErrorCode.InvalidInput);
            exception.Type.Should().Be(InputType.ImageFormat);
        }

        [Test]
        public void LayoutImageConverter_CreateWallpaper_RequiredImagesAreMissing_ThrowToolException()
        {
            Mock.Get(_augmentedImageFileProvider)
                .Setup(x => x.GetFilesFrom(It.IsAny<string>()))
                .Returns(new List<string>());

            Mock.Get(_augmentedImageFileProvider)
                .Setup(x => x.CheckEveryNeededFileArePresent(It.IsAny<string>()))
                .Returns(false);

            var imageType = new ImageType("Fake", new List<string>() { Constantes.NoneModifier });
            var informations = new ImageConversionInformation("95", @"C:\this\is\a\fake\path\file.png", "1bpp", imageType);
            var layoutImageConverter = new LayoutImageConverter(informations, _augmentedImageFileProvider, _pathProvider);

            var exception = Assert.Throws<InvalidInputException>(() =>
            {
                layoutImageConverter.CreateWallpaper();
            });

            exception.ErrorCode.Should().Be(ToolErrorCode.InvalidInput);
            exception.Type.Should().Be(InputType.FolderContent);
        }

        [Test]
        public void LayoutImageConverter_CreateWallpaper_Azerty_Hide_Ok() => Test_CreateWallpaper_Azerty_WithType("Hide");

        [Test]
        public void LayoutImageConverter_CreateWallpaper_Azerty_Bold_Ok() => Test_CreateWallpaper_Azerty_WithType("Bold");

        [Test]
        public void LayoutImageConverter_CreateWallpaper_Azerty_Classic_Ok() => Test_CreateWallpaper_Azerty_WithType("Classic");

        [Test]
        public void LayoutImageConverter_CreateWallpaper_Azerty_Classic_WithMoreThanNeededImages_Ok()
        {
            var layoutId = "67896332";
            var imageTypeName = "classic";

            var testDirectory = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Resources",
                "AzertySample",
                "Classic_TooManyImages"
            );

            var samplePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Resources",
                "AzertySample",
                $"{layoutId}_{imageTypeName}.wallpaper.gz"
            );

            var imageType = new ImageTypeFactory().CreateImageType(imageTypeName);
            var information = new ImageConversionInformation(layoutId, testDirectory, "1bpp", imageType);

            TestLayoutImageConversion(information, samplePath);
        }

        private void Test_CreateWallpaper_Azerty_WithType(string folderName)
        {
            var layoutId = "67896332";
            var imageTypeName = folderName.ToLower();

            var testDirectory = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Resources",
                "AzertySample",
                folderName
            );

            var samplePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Resources",
                "AzertySample",
                $"{layoutId}_{imageTypeName}.wallpaper.gz"
            );

            var imageType = new ImageTypeFactory().CreateImageType(imageTypeName);
            var information = new ImageConversionInformation(layoutId, testDirectory, "1bpp", imageType);

            TestLayoutImageConversion(information, samplePath);
        }

        private void TestLayoutImageConversion(ImageConversionInformation conversionInformation, string sampleFilePath)
        {
            var layoutId = conversionInformation.LayoutId;
            var imageTypeName = conversionInformation.ImageType.TypeName;

            var wallpaperOutputFolder = Path.Combine(Directory.GetCurrentDirectory(), "HID");
            var wallpaperFilePath = Path.Combine(wallpaperOutputFolder, $"{layoutId}_{imageTypeName}.wallpaper.gz");

            var layoutImageConverter = new LayoutImageConverter(
                conversionInformation,
                new AugmentedImageFileProvider(conversionInformation.ImageType),
                _pathProvider
            );

            layoutImageConverter.CreateWallpaper();

            File.Exists(wallpaperFilePath).Should().BeTrue();

            var generatedContent = File.ReadAllBytes(wallpaperFilePath);
            var sampleContent = File.ReadAllBytes(sampleFilePath);

            generatedContent.Should().BeEquivalentTo(sampleContent);

            Directory.Delete(wallpaperOutputFolder, true);
        }
    }
}
