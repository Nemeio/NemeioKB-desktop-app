using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Nemeio.Tools.LayoutConverter.Exceptions;
using Nemeio.Tools.LayoutConverter.Models;
using NUnit.Framework;

namespace Nemeio.Tools.LayoutConverter.Tests.Models
{
    public class ImageConversionInformationShould
    {
        private ImageType _imageType;

        [SetUp]
        public void SetUp()
        {
            _imageType = new ImageType("Fake", new List<string>() { Constantes.NoneModifier });
        }

        [Test]
        public void ImageConversionInformation_Constructor_Ok()
        {
            var layoutId = "67896332";
            var selectedFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "myImages");
            var imageFormat = "1bpp";
            var imageType = new ImageType("classic", new List<string>() { Constantes.NoneModifier });

            Assert.DoesNotThrow(() => new ImageConversionInformation(layoutId, selectedFolderPath, imageFormat, imageType));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void ImageConversionInformation_Constructor_NullOrEmptyParameter_ThrowsInvalidInputException(string layoutId)
        {
            var exception = Assert.Throws<InvalidInputException>(() => new ImageConversionInformation(layoutId, @"C:\this\is\a\fake\path\file.png", "1bpp", _imageType));

            exception.ErrorCode.Should().Be(ToolErrorCode.InvalidInput);
            exception.Type.Should().Be(InputType.LayoutId);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void ImageConversionInformation_Constructor_SelectedFolderPathIsNullOrEmpty_ThrowsInvalidInputException(string folderPath)
        {
            var exception = Assert.Throws<InvalidInputException>(() => new ImageConversionInformation("95", folderPath, "1bpp", _imageType));

            exception.ErrorCode.Should().Be(ToolErrorCode.InvalidInput);
            exception.Type.Should().Be(InputType.FolderPath);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void ImageConversionInformation_Constructor_ImageFormatIsNullOrEmpty_ThrowsInvalidInputException(string format)
        {
            var exception = Assert.Throws<InvalidInputException>(() => new ImageConversionInformation("95", @"C:\this\is\a\fake\path\file.png", format, _imageType));

            exception.ErrorCode.Should().Be(ToolErrorCode.InvalidInput);
            exception.Type.Should().Be(InputType.ImageFormat);
        }
    }
}
