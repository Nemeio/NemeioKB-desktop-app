using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
using Nemeio.Core.Models.Fonts;
using Nemeio.LayoutGen.Models;
using NUnit.Framework;

namespace Nemeio.LayoutGen.Test
{
    [TestFixture]
    public class LGTypefaceProviderShould
    {
        private LGTypefaceProvider _typefaceProvider;
        private IFontProvider _mockFontProvider;

        [SetUp]
        public void SetUp()
        {
            _mockFontProvider = Mock.Of<IFontProvider>();

            Mock.Get(_mockFontProvider)
                .Setup(x => x.Fonts)
                .Returns(new HashSet<FontInfo>()
                {
                    new FontInfo("Font1", 0, new MemoryStream()),
                    new FontInfo("Font2", 1, new MemoryStream()),
                    new FontInfo("Font3", 2, new MemoryStream())
                });

            _typefaceProvider = new LGTypefaceProvider(_mockFontProvider);
        }

        [Test]
        public void LGTypefaceProvider_01_01_Constructor()
        {
            var mockFontProvider = Mock.Of<IFontProvider>();

            Mock.Get(mockFontProvider)
                .Setup(x => x.Fonts)
                .Returns(new HashSet<FontInfo>()
                {
                    new FontInfo("Font1", 0, new MemoryStream()),
                    new FontInfo("Font2", 1, new MemoryStream()),
                    new FontInfo("Font3", 2, new MemoryStream())
                });

            Assert.DoesNotThrow(() => new LGTypefaceProvider(mockFontProvider));
            Assert.Throws<ArgumentNullException>(() => new LGTypefaceProvider(null));
        }

        [Test]
        public void LGTypefaceProvider_01_02_Constructor_LoadTypefaces()
        {
            _typefaceProvider.Typefaces.Count.Should().Be(_mockFontProvider.Fonts.Count());
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void LGTypefaceProvider_02_01_GetTypefaceByName_WithInvalidParameter_ThrowInvalidOperationException(string fontName)
        {
            Assert.Throws<InvalidOperationException>(() => _typefaceProvider.GetTypefaceByName(fontName));
        }
    }
}
