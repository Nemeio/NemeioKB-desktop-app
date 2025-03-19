using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Errors;
using Nemeio.Core.Models.Fonts;
using Nemeio.LayoutGen.Models;
using Nemeio.Models.Fonts;
using NUnit.Framework;

namespace Nemeio.LayoutGen.Test
{
    [TestFixture]
    public class LGFontSelectorShould
    {
        private ILoggerFactory _loggerFactory;
        private IErrorManager _mockErrorManager;
        private IFontProvider _fontProvider;
        private IFontSelector _fontSelector;

        [SetUp]
        public void SetUp()
        {
            _loggerFactory = new LoggerFactory();

            _mockErrorManager = Mock.Of<IErrorManager>();

            Mock.Get(_mockErrorManager)
                .Setup(x => x.GetFullErrorMessage(It.IsAny<ErrorCode>(), It.IsAny<Exception>()))
                .Returns(string.Empty);

            _fontProvider = new FontProvider(_loggerFactory, _mockErrorManager);
            _fontSelector = new LGFontSelector(_fontProvider, new LGTypefaceProvider(_fontProvider));
        }

        [Test]
        public void LGFontSelector_01_01_Constructor_WorksOk()
        {
            var fontProvider = new FontProvider(_loggerFactory, _mockErrorManager);

            Assert.DoesNotThrow(() => new LGFontSelector(Mock.Of<IFontProvider>(), new LGTypefaceProvider(fontProvider)));

            fontProvider = new FontProvider(_loggerFactory, _mockErrorManager);

            Assert.Throws<ArgumentNullException>(() => new LGFontSelector(null, new LGTypefaceProvider(fontProvider)));
            Assert.Throws<ArgumentNullException>(() => new LGFontSelector(Mock.Of<IFontProvider>(), null));
        }

        [Test]
        public void LGFontSelector_02_01_FallbackFontIfNeeded_WithNullParameters_ThrowArgumentNullException()
        {
            var fontProvider = new FontProvider(_loggerFactory, _mockErrorManager);
            var fontSelector = new LGFontSelector(Mock.Of<IFontProvider>(), new LGTypefaceProvider(fontProvider));

            Assert.Throws<ArgumentNullException>(() => fontSelector.FallbackFontIfNeeded(null, "not_null_string"));
            Assert.Throws<ArgumentNullException>(() => fontSelector.FallbackFontIfNeeded(FontProvider.GetDefaultFont(), null));
        }

        [Test]
        public void LGFontSelector_02_02_FallbackFontIfNeeded_WithUnknownFont_ThrowsInvalidOperationException()
        {
            var unknownFont = new Font("this_is_an_unknown_font", FontSize.Medium, false, false);

            Assert.Throws<InvalidOperationException>(() => _fontSelector.FallbackFontIfNeeded(unknownFont, "A"));
        }

        [TestCase("ة")]
        [TestCase("أَ")]
        [TestCase("ف")]
        public void LGFontSelector_02_03_FallbackFontIfNeeded_ForArabicCharacter_WorksOk(string character)
        {
            FallbackFontMustBeEquals(character, NemeioConstants.Cairo);
        }

        [TestCase("平")]
        [TestCase("片")]
        [TestCase("漢")]
        public void LGFontSelector_02_04_FallbackFontIfNeeded_ForJapaneseCharacter_WorksOk(string character)
        {
            FallbackFontMustBeEquals(character, NemeioConstants.NotoJP);
        }

        [TestCase("뚜")]
        [TestCase("씨")]
        [TestCase("호")]
        public void LGFontSelector_02_05_FallbackFontIfNeeded_ForKoreanCharacter_WorksOk(string character)
        {
            FallbackFontMustBeEquals(character, NemeioConstants.NotoKR);
        }

        [TestCase("₽")]
        public void LGFontSelector_02_06_FallbackFontIfNeeded_ForCyrillicCharacter_WorksOk(string character)
        {
            FallbackFontMustBeEquals(character, NemeioConstants.Roboto);
        }

        private void FallbackFontMustBeEquals(string character, string waitedFontName)
        {
            var notoRegularFont = new Font(NemeioConstants.Noto, FontSize.Medium, false, false);

            var selectedFont = _fontSelector.FallbackFontIfNeeded(notoRegularFont, character);

            selectedFont.Name.Should().Be(waitedFontName);
        }
    }
}
