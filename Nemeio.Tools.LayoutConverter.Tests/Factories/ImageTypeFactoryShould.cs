using System;
using System.Linq;
using FluentAssertions;
using Nemeio.Tools.LayoutConverter.Factories;
using NUnit.Framework;

namespace Nemeio.Tools.LayoutConverter.Tests.Factories
{
    public class ImageTypeFactoryShould
    {
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void ImageTypeFactory_CreateImageType_WithInvalidParameter_ThrowsArgumentNullException(string type)
        {
            var factory = new ImageTypeFactory();

            Assert.Throws<ArgumentNullException>(() => factory.CreateImageType(type));
        }

        [TestCase("this_is_not_a_type")]
        [TestCase("ldlc")]
        public void ImageTypeFactory_CreateImageType_WithUnknownType_ThrowsInvalidOperationException(string type)
        {
            var factory = new ImageTypeFactory();

            Assert.Throws<InvalidOperationException>(() => factory.CreateImageType(type));
        }

        [TestCase(ImageTypeFactory.ClassicImageName)]
        [TestCase(ImageTypeFactory.GrayImageName)]
        [TestCase(ImageTypeFactory.HideImageName)]
        public void ImageTypeFactory_CreateImageType_WithValidTypeName_Ok(string type)
        {
            var factory = new ImageTypeFactory();

            Assert.DoesNotThrow(() => factory.CreateImageType(type));
        }

        [Test]
        public void ImageTypeFactory_WithGrayType_Ok()
        {
            var factory = new ImageTypeFactory();
            var result = factory.CreateImageType(ImageTypeFactory.GrayImageName);

            result.SupportedModifiers.Should().NotBeNullOrEmpty();
            result.SupportedModifiers.Count().Should().Be(6);
            result.TypeName.Should().Be(ImageTypeFactory.GrayImageName);
        }

        [Test]
        public void ImageTypeFactory_WithHideType_Ok()
        {
            var factory = new ImageTypeFactory();
            var result = factory.CreateImageType(ImageTypeFactory.HideImageName);

            result.SupportedModifiers.Should().NotBeNullOrEmpty();
            result.SupportedModifiers.Count().Should().Be(6);
            result.TypeName.Should().Be(ImageTypeFactory.HideImageName);
        }

        [Test]
        public void ImageTypeFactory_WithClassicType_Ok()
        {
            var factory = new ImageTypeFactory();
            var result = factory.CreateImageType(ImageTypeFactory.ClassicImageName);

            result.SupportedModifiers.Should().NotBeNullOrEmpty();
            result.SupportedModifiers.Count().Should().Be(1);
            result.TypeName.Should().Be(ImageTypeFactory.ClassicImageName);
        }
    }
}
