using System;
using System.Linq;
using FluentAssertions;
using Nemeio.Tools.LayoutConverter.Models.Requirements;
using NUnit.Framework;

namespace Nemeio.Tools.LayoutConverter.Tests.Models
{
    public class ImageFormatRequirementShould
    {
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void ImageFormatRequirement_Check_WithInvalidParameter_ThrowsArgumentNullException(string filePath)
        {
            var imageFormatRequirement = new ImageFormatRequirement();

            Assert.Throws<ArgumentNullException>(() => imageFormatRequirement.Check(filePath));
        }

        [Test]
        public void ImageFormatRequirement_Check_Ok()
        {
            var imageFormatRequirement = new ImageFormatRequirement();

            var pngResult = imageFormatRequirement.Check("mon_fichier.png");
            pngResult.Should().BeNull();

            var notPngResult = imageFormatRequirement.Check("mon_fichier.bmp");
            notPngResult.Should().NotBeNull();
            notPngResult.Count().Should().Be(1);
        }
    }
}
