using System;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Settings.Parser;
using Nemeio.Core.Settings.Types;

namespace Nemeio.Core.Settings.Providers
{
    public sealed class SettingsProvider : ISettingsProvider
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISettingsParser _parser;

        public SettingsProvider(ILoggerFactory loggerFactory, ISettingsParser parser)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        public ISettings LoadSettings(string filePath)
        {
            var logger = _loggerFactory.CreateLogger<SettingsProvider>();
            var readedSettings = _parser.Parse(filePath);

            var settings = new Settings(
                new SwaggerEnableSetting(logger, readedSettings.SwaggerEnable),
                new ApiPortSetting(logger, ToNullableInt(readedSettings.ApiPort)),
                new EnvironmentSetting(logger, readedSettings.Environment),
                new JpegCompressionLevelSetting(logger, readedSettings.JpegCompressionPercent),
                new AutoStartWebServerSetting(logger, readedSettings.AutoStartWebServer)
            );
            return settings;
        }

        private int? ToNullableInt(string value)
        {
            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }
            return null;
        }
    }
}
