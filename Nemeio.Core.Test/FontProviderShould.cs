using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Errors;
using Nemeio.Core.Models.Fonts;
using NUnit.Framework;

namespace Nemeio.Core.Test
{
    [TestFixture]
    public class FontProviderShould
    {
        private ILoggerFactory _loggerFactory;
        private IErrorManager _mockErrorManager;

        [SetUp]
        public void SetUp()
        {
            _loggerFactory = new LoggerFactory();

            _mockErrorManager = Mock.Of<IErrorManager>();

            Mock.Get(_mockErrorManager)
                .Setup(x => x.GetFullErrorMessage(It.IsAny<ErrorCode>(), It.IsAny<Exception>()))
                .Returns(string.Empty);
        }

        [Test]
        public void FontProvider_01_01_Constructor_WorksOk()
        {
            Assert.DoesNotThrow(() => new FontProvider(_loggerFactory, _mockErrorManager));
        }

        [Test]
        public void FontProvider_01_02_Constructor_WithNullParameters_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new FontProvider(null, _mockErrorManager));
            Assert.Throws<ArgumentNullException>(() => new FontProvider(_loggerFactory, null));
        }

        [Test]
        public void FontProvider_01_03_Constructor_AutomaticLoadFonts_PopulateFontProviderFonts()
        {
            var fontProvider = new FontProvider(_loggerFactory, _mockErrorManager);

            fontProvider.Fonts.Count().Should().BeGreaterThan(0);
        }

        [Test]
        public void FontProvider_02_01_RegisterFont_NullParameter_ThrowsArgumentNullException()
        {
            var fontProvider = new FontProvider(_loggerFactory, _mockErrorManager);

            Assert.Throws<ArgumentNullException>(() => fontProvider.RegisterFont(null));
        }

        [Test]
        public void FontProvider_02_02_RegisterFont_PriorityAlreadyUsed_ThrowsInvalidDataException()
        {
            using (var memoryStream = new MemoryStream())
            {
                const int notoSansPriority = 0;

                var fontProvider = new FontProvider(_loggerFactory, _mockErrorManager);
                var fontInfo = new FontInfo(string.Empty, notoSansPriority, memoryStream);

                Assert.Throws<InvalidDataException>(() => fontProvider.RegisterFont(fontInfo));
            }
        }

        [Test]
        public void FontProvider_02_03_RegisterFont_WithNewFont_AddFontToFontProviderList()
        {
            using (var memoryStream = new MemoryStream())
            {
                var fontProvider = new FontProvider(_loggerFactory, _mockErrorManager);
                var beforeRegisterCount = fontProvider.Fonts.Count();

                var fontInfo = new FontInfo(string.Empty, 999, memoryStream);
                fontProvider.RegisterFont(fontInfo);

                var afterRegisterCount = fontProvider.Fonts.Count();

                afterRegisterCount.Should().Be(beforeRegisterCount + 1);
            }
        }

        [Test]
        public void FontProvider_03_01_FontExists_WithUnknownFont_ReturnFalse()
        {
            var fontProvider = new FontProvider(_loggerFactory, _mockErrorManager);

            var exists = fontProvider.FontExists("this_font_doesn't_exists.ttf");

            exists.Should().BeFalse();
        }

        [Test]
        public void FontProvider_03_02_FontExists_WithExistsFont_ReturnTrue()
        {
            var fontProvider = new FontProvider(_loggerFactory, _mockErrorManager);

            var exists = fontProvider.FontExists(NemeioConstants.Noto);

            exists.Should().BeTrue();
        }
    }
}
