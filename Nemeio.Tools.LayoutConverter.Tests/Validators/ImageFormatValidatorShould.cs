using System;
using System.Linq;
using FluentAssertions;
using Nemeio.Tools.LayoutConverter.Validators;
using NUnit.Framework;

namespace Nemeio.Tools.LayoutConverter.Tests.Validators
{
    public class ImageFormatValidatorShould
    {
        [Test]
        public void ImageFormatValidator_SupportedFormat_MustBeEqualToTwo()
        {
            ImageFormatValidator.SupportedFormats.Count().Should().Be(2);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void ImageFormatValidator_IsValidFormat_WithInvalidParameter_ReturnFalse(string invalidParameter)
        {
            var imageFormatValidator = new ImageFormatValidator();
            var result = imageFormatValidator.IsValidFormat(invalidParameter);

            result.Should().BeFalse();
        }

        [TestCase("1bpp")]
        [TestCase("2bpp")]
        public void ImageFormatValidator_IsValidFormat_WithSupportedFormat_ReturnTrue(string format)
        {
            var imageFormatValidator = new ImageFormatValidator();
            var result = imageFormatValidator.IsValidFormat(format);

            result.Should().BeTrue();
        }

        [TestCase("4bpp")]
        [TestCase("16bpp")]
        public void ImageFormatValidator_IsValidFormat_WithUnknownFormat_ReturnFalse(string format)
        {
            var imageFormatValidator = new ImageFormatValidator();
            var result = imageFormatValidator.IsValidFormat(format);

            result.Should().BeFalse();
        }

        [TestCase("1bpp")]
        [TestCase("2bpp")]
        public void ImageFormatValidator_GetFormatByName_WithSupportedFormat_NotThrowsException(string format)
        {
            var imageFormatValidator = new ImageFormatValidator();

            Assert.DoesNotThrow(() => 
            {
                imageFormatValidator.GetFormatByName(format);
            });
        }

        [Test]
        public void ImageFormatValidator_GetFormatByName_With1bpp_ReturnOneBitPerPixel() => IsValidFormat("1bpp", ImageFormat.OneBitPerPixel);
        
        [Test]
        public void ImageFormatValidator_GetFormatByName_With2bpp_ReturnTwoBitPerPixel() => IsValidFormat("2bpp", ImageFormat.TwoBitsPerPixel);

        [TestCase("4bpp")]
        [TestCase("16bpp")]
        public void ImageFormatValidator_GetFormatByName_WithUnsupportedFormat_ThrowsInvalidOperationException(string format)
        {
            var imageFormatValidator = new ImageFormatValidator();

            Assert.Throws<InvalidOperationException>(() =>
            {
                imageFormatValidator.GetFormatByName(format);
            });
        }

        private bool IsValidFormat(string inputFormat, ImageFormat outputFormatWanted)
        {
            var imageFormatValidator = new ImageFormatValidator();
            var result = imageFormatValidator.GetFormatByName(inputFormat);

            return result == outputFormatWanted;
        }
    }
}
