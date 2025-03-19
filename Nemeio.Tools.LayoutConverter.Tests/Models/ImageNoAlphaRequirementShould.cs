using System.IO;
using System.Linq;
using FluentAssertions;
using Nemeio.Tools.LayoutConverter.Models.Requirements;
using NUnit.Framework;

namespace Nemeio.Tools.LayoutConverter.Tests.Models
{
    public class ImageNoAlphaRequirementShould
    {
        [Test]
        public void ImageNoAlphaRequirement_Check_WithImageWhichContainAlpha_ReturnRequirementError()
        {
            var imageNoAlphaRequirement = new ImageNoAlphaRequirement();

            var alphaImagePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Resources",
                "jp_layout_with_alpha.png"
            );

            var result = imageNoAlphaRequirement.Check(alphaImagePath);

            result.Should().NotBeNull();
            result.Count().Should().Be(1);
        }

        [Test]
        public void ImageNoAlphaRequirement_Check_WithValidImage_ReturnNull()
        {
            var imageNoAlphaRequirement = new ImageNoAlphaRequirement();

            var alphaImagePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Resources",
                "jp_layout_without_alpha.png"
            );

            var result = imageNoAlphaRequirement.Check(alphaImagePath);

            result.Should().BeNull();
        }
    }
}
