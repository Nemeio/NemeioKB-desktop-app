using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Nemeio.Tools.LayoutConverter.Models.Requirements;
using NUnit.Framework;

namespace Nemeio.Tools.LayoutConverter.Tests.Models
{
    public class ImageSizeRequirementShould
    {
        [TestCase("jp_layout_with_alpha.png")]
        [TestCase("jp_layout_without_alpha.png")]
        public void ImageSizeRequirement_Check_WithValidImage_ReturnNull(string imageName)
        {
            var result = TestWithImage(imageName);

            result.Should().BeNull();
        }

        [Test]
        public void ImageSizeRequirement_Check_WithInvalidWidth_ReturnRequirementError()
        {
            var result = TestWithImage("layout_invalid_width.png");

            result.Should().NotBeNull();
            result.Count().Should().Be(1);
        }

        [Test]
        public void ImageSizeRequirement_Check_WithInvalidHeight_ReturnRequirementError()
        {
            var result = TestWithImage("layout_invalid_height.png");

            result.Should().NotBeNull();
            result.Count().Should().Be(1);
        }

        [Test]
        public void ImageSizeRequirement_Check_WithInvalidWidthAndHeight_ReturnRequirementErrors()
        {
            var result = TestWithImage("layout_invalid_width_and_height.png");

            result.Should().NotBeNull();
            result.Count().Should().Be(2);
        }

        private IEnumerable<RequirementError> TestWithImage(string imageName)
        {
            var imageSizeRequirement = new ImageSizeRequirement();

            var imagePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Resources",
                imageName
            );

            return imageSizeRequirement.Check(imagePath);
        }
    }
}
