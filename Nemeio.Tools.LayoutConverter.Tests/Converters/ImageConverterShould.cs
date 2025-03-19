using System;
using System.IO;
using FluentAssertions;
using Nemeio.Tools.LayoutConverter.Converters;
using Nemeio.Tools.LayoutConverter.Validators;
using NUnit.Framework;

namespace Nemeio.Tools.LayoutConverter.Tests.Converters
{
    internal class ImageConverterShould
    {
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void ImageConverter_Convert_InvalidParameter_ThrowsArgumentNullException(string filePath)
        {
            var imageConverter = new ImageConverter(ImageFormat.OneBitPerPixel);

            Assert.Throws<ArgumentNullException>(() => imageConverter.Convert(filePath));
        }

        [Test]
        public void ImageConverter_Convert_1bpp_Ok()
        {
            var testImagePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Resources",
                "AzertySample",
                "Classic",
                "67896332_none_classic.png"
            );

            var testResultImagePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Resources",
                "image_converted_to_one_bpp.1bpp"
            );

            var imageConverter = new ImageConverter(ImageFormat.OneBitPerPixel);
            var oneBppImageConverted = imageConverter.Convert(testImagePath);

            var sampleContent = File.ReadAllBytes(testResultImagePath);

            oneBppImageConverted.Should().BeEquivalentTo(sampleContent);
        }
    }
}
