using System;
using FluentAssertions;
using Moq;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Versions;
using Nemeio.Windows.Application.Applications;
using Nemeio.Wpf.Services;
using NUnit.Framework;

namespace Nemeio.Wpf.Test
{
    public class WinInformationServiceShould
    {
        [Test]
        public void WinInformationService_Constructor_Ok()
        {
            var mockFontProvider = Mock.Of<IFontProvider>();
            var mockApplicationVersionProvider = Mock.Of<IApplicationVersionProvider>();

            Assert.Throws<ArgumentNullException>(() => new WinInformationService(null, mockApplicationVersionProvider));
            Assert.Throws<ArgumentNullException>(() => new WinInformationService(mockFontProvider, null));
            Assert.DoesNotThrow(() => new WinInformationService(mockFontProvider, mockApplicationVersionProvider));
        }

        [Test]
        public void WinInformationService_GetApplicationVersion_With4DigitsVersion_Return2DigitVersion()
        {
            var mockFontProvider = Mock.Of<IFontProvider>();
            var mockApplicationVersionProvider = Mock.Of<IApplicationVersionProvider>();

            Mock.Get(mockApplicationVersionProvider)
                .Setup(x => x.GetVersion())
                .Returns(new Version("0.2.48.1234"));

            var informationService = new WinInformationService(mockFontProvider, mockApplicationVersionProvider);
            var version = informationService.GetApplicationVersion();

            version.ToString().Should().Be("0.2");
        }

        [Test]
        public void WinInformationService_GetApplicationVersion_With2DigitsVersion_Return2DigitVersion()
        {
            var mockFontProvider = Mock.Of<IFontProvider>();
            var mockApplicationVersionProvider = Mock.Of<IApplicationVersionProvider>();

            Mock.Get(mockApplicationVersionProvider)
                .Setup(x => x.GetVersion())
                .Returns(new Version("48.3"));

            var informationService = new WinInformationService(mockFontProvider, mockApplicationVersionProvider);
            var version = informationService.GetApplicationVersion();

            version.ToString().Should().Be("48.3");
        }
    }
}
