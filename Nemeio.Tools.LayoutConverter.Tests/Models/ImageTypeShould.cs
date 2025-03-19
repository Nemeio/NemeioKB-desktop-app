using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Nemeio.Tools.LayoutConverter.Models;
using NUnit.Framework;

namespace Nemeio.Tools.LayoutConverter.Tests.Models
{
    public class ImageTypeShould
    {
        [Test]
        public void ImageType_Constructor_Ok()
        {
            Assert.DoesNotThrow(() => new ImageType("classic", new List<string>() { Constantes.NoneModifier }));
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void ImageType_Constructor_WithInvalidTypeName_ThrowsArgumentNullException(string typeName)
        {
            Assert.Throws<ArgumentNullException>(() => new ImageType(typeName, new List<string>() { Constantes.NoneModifier }));
        }

        [Test]
        public void ImageType_Constructor_WithNullSupportedModifier_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ImageType("this_is_my_name", null));
        }

        [Test]
        public void ImageType_Constructor_WithUnsupportedCountSupportedModifier_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ImageType("this_is_my_name", new List<string>()));
        }

        [Test]
        public void ImageType_ComposeFileName_Ok()
        {
            const string imageTypeName      = "test";
            const string layoutId           = "my_layout_id";
            const string modifier           = "my_modifier";
            const string divider            = "_";

            var imageType = new ImageType(imageTypeName, new List<string>() { Constantes.NoneModifier });

            var result = imageType.ComposeFileName(layoutId, modifier);

            result.Should().Contain(imageTypeName);
            result.Should().Contain(layoutId);
            result.Should().Contain(modifier);
            result.Should().Contain(divider);

            Path.GetExtension(result).Should().Be(".png");
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void ImageType_ComposeFileName_WithInvalidLayoutId_ThrowsArgumentNullException(string layoutId)
        {
            var imageType = new ImageType("test", new List<string>() { Constantes.NoneModifier });

            Assert.Throws<ArgumentNullException>(() => imageType.ComposeFileName(layoutId, "my_modifier"));
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void ImageType_ComposeFileName_WithInvalidModifier_ThrowsArgumentNullException(string modifier)
        {
            var imageType = new ImageType("test", new List<string>() { Constantes.NoneModifier });

            Assert.Throws<ArgumentNullException>(() => imageType.ComposeFileName("my_id", modifier));
        }
    }
}
