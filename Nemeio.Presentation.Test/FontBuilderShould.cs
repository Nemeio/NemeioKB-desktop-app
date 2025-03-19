using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Nemeio.Core;
using Nemeio.Core.Models.Fonts;
using Nemeio.Layout.Builder.Builders;
using Nemeio.Models.Fonts;
using NUnit.Framework;

namespace Nemeio.Presentation.Test
{
    [TestFixture]
    public class FontBuilderShould
    {
        private const string mainFontName = "MyCustomFont";

        private IFontProvider _mockFontProvider;

        [SetUp]
        public void SetUp()
        {
            _mockFontProvider = Mock.Of<IFontProvider>();

            var fontStream = typeof(NemeioConstants).Assembly.GetManifestResourceStream("Nemeio.Core.Resources.Fonts.NotoSans-Regular.ttf");

            Mock.Get(_mockFontProvider)
                .Setup(x => x.Fonts)
                .Returns(new HashSet<FontInfo>()
                {
                    new FontInfo(mainFontName, 0, fontStream)
                });
        }

        [Test]
        public void FontBuilder_01_01_Constructor_WithNullParameter_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new FontBuilder(null));
        }

        [Test]
        public void FontBuilder_01_02_Constructor_RetrieveMainFont()
        {
            var fontBuilder = new TestableFontBuilder(_mockFontProvider);

            fontBuilder.FontName.Should().Be(mainFontName);
            fontBuilder.IsFunctionKey.Should().BeFalse();
            fontBuilder.IsRequiredKey.Should().BeFalse();
            fontBuilder.FontIsBold.Should().BeFalse();
            fontBuilder.FontIsItalic.Should().BeFalse();
            fontBuilder.FontSize.Should().Be(FontSize.Medium);
        }

        [Test]
        public void FontBuilder_02_01_AdjustIfSpecialKey_WhenRequiredAndNotFunction_ReduceFontSize()
        {
            var fontBuilder = new TestableFontBuilder(_mockFontProvider);
            fontBuilder.SetIsRequiredKey(true);
            fontBuilder.SetIsFunctionKey(false);

            var sizeBefore = fontBuilder.FontSize;
            sizeBefore.Should().Be(FontSize.Medium);

            fontBuilder.AdjustIfSpecialKey();

            var sizeAfter = fontBuilder.FontSize;
            sizeAfter.Should().Be(FontSize.Small);
        }

        [Test]
        public void FontBuilder_02_02_AdjustIfSpecialKey_WhenRequiredAndFunction_DoNothing()
        {
            var fontBuilder = new TestableFontBuilder(_mockFontProvider);
            fontBuilder.SetIsRequiredKey(true);
            fontBuilder.SetIsFunctionKey(true);

            var sizeBefore = fontBuilder.FontSize;
            sizeBefore.Should().Be(FontSize.Medium);

            fontBuilder.AdjustIfSpecialKey();

            var sizeAfter = fontBuilder.FontSize;
            sizeAfter.Should().Be(FontSize.Medium);
        }

        [Test]
        public void FontBuilder_02_03_AdjustIfSpecialKey_WhenNotRequiredAndFunction_DoNothing()
        {
            var fontBuilder = new TestableFontBuilder(_mockFontProvider);
            fontBuilder.SetIsRequiredKey(false);
            fontBuilder.SetIsFunctionKey(true);

            var sizeBefore = fontBuilder.FontSize;
            sizeBefore.Should().Be(FontSize.Medium);

            fontBuilder.AdjustIfSpecialKey();

            var sizeAfter = fontBuilder.FontSize;
            sizeAfter.Should().Be(FontSize.Medium);
        }

        [Test]
        public void FontBuilder_02_04_AdjustIfSpecialKey_WhenNotRequiredAndNotFunction_DoNothing()
        {
            var fontBuilder = new TestableFontBuilder(_mockFontProvider);
            fontBuilder.SetIsRequiredKey(false);
            fontBuilder.SetIsFunctionKey(false);

            var sizeBefore = fontBuilder.FontSize;
            sizeBefore.Should().Be(FontSize.Medium);

            fontBuilder.AdjustIfSpecialKey();

            var sizeAfter = fontBuilder.FontSize;
            sizeAfter.Should().Be(FontSize.Medium);
        }

        [Test]
        public void FontBuilder_03_01_Build_CreateFont()
        {
            var fontBuilder = new TestableFontBuilder(_mockFontProvider);
            var font = fontBuilder.Build();

            font.Should().NotBeNull();
            font.Name.Should().Be(fontBuilder.FontName);
            font.Size.Should().Be(fontBuilder.FontSize);
            font.Bold.Should().Be(fontBuilder.FontIsBold);
            font.Italic.Should().Be(fontBuilder.FontIsItalic);
        }

        private class TestableFontBuilder : FontBuilder
        {
            public TestableFontBuilder(IFontProvider fontProvider) 
                : base(fontProvider) { }

            public bool IsRequiredKey => _isRequiredKey;

            public bool IsFunctionKey => _isFunctionKey;

            public string FontName => _fontName;

            public bool FontIsBold => _fontIsBold;

            public bool FontIsItalic => _fontIsItalic;

            public FontSize FontSize => _fontSize;
        }
    }
}
