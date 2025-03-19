using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.DataModels;
using Nemeio.Core.Settings.Parser;
using Nemeio.Core.Settings.Providers;
using NUnit.Framework;

namespace Nemeio.Core.Test.Settings
{
    [TestFixture]
    internal class SettingsProviderShould
    {
        [Test]
        public void SettingsProvider_LoadSettings_Ok()
        {
            const string apiPortString = "1234";

            const string environment = "master";
            const int jpegCompressionLevel = 74;
            const bool swaggerState = true;
            int apiPort = int.Parse(apiPortString);

            var filePath = @"C:\Users\username\this\is\my\path.xml";
            var parserResult = new DevelopmentSettings()
            {
                ApiPort = apiPortString,
                Environment = environment,
                JpegCompressionPercent = jpegCompressionLevel,
                SwaggerEnable = swaggerState
            };
            
            var parser = Mock.Of<ISettingsParser>();
            Mock.Get(parser)
                .Setup(x => x.Parse(It.IsAny<string>()))
                .Returns(parserResult);

            var provider = new SettingsProvider(new LoggerFactory(), parser);

            var settings = provider.LoadSettings(filePath);

            settings.Should().NotBeNull();

            settings.SwaggerEnable.Should().NotBeNull();
            settings.SwaggerEnable.Value.Should().NotBeNull();
            settings.SwaggerEnable.Value.Should().Be(swaggerState);

            settings.ApiPort.Should().NotBeNull();
            settings.ApiPort.Value.Should().NotBeNull();
            settings.ApiPort.Value.Should().Be(apiPort);

            settings.EnvironmentSetting.Should().NotBeNull();
            settings.EnvironmentSetting.Value.Should().NotBeNull();
            settings.EnvironmentSetting.Value.Should().Be(environment);

            settings.JpegCompressionLevelSetting.Should().NotBeNull();
            settings.JpegCompressionLevelSetting.Value.Should().NotBeNull();
            settings.JpegCompressionLevelSetting.Value.Should().Be(jpegCompressionLevel);
        }
    }
}
