using System;
using Nemeio.Core.Settings.Types;

namespace Nemeio.Core.Settings
{
    public sealed class Settings : ISettings
    {
        public SwaggerEnableSetting SwaggerEnable { get; private set; }
        public ApiPortSetting ApiPort { get; private set; }
        public EnvironmentSetting EnvironmentSetting { get; private set; }
        public JpegCompressionLevelSetting JpegCompressionLevelSetting { get; private set; }
        public AutoStartWebServerSetting AutoStartWebServerSetting { get; private set; }

        public Settings(SwaggerEnableSetting swaggerEnable, ApiPortSetting apiPort, EnvironmentSetting environmentSetting, JpegCompressionLevelSetting jpegCompressionLevelSetting, AutoStartWebServerSetting autoStartWebServerSetting)
        {
            SwaggerEnable = swaggerEnable ?? throw new ArgumentNullException(nameof(swaggerEnable));
            ApiPort = apiPort ?? throw new ArgumentNullException(nameof(apiPort));
            EnvironmentSetting = environmentSetting ?? throw new ArgumentNullException(nameof(environmentSetting));
            JpegCompressionLevelSetting = jpegCompressionLevelSetting ?? throw new ArgumentNullException(nameof(jpegCompressionLevelSetting));
            AutoStartWebServerSetting = autoStartWebServerSetting ?? throw new ArgumentNullException(nameof(autoStartWebServerSetting));
        }

        public void Update(ISettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            SwaggerEnable.Value = settings.SwaggerEnable.Value;
            ApiPort.Value = settings.ApiPort.Value;
            EnvironmentSetting.Value = settings.EnvironmentSetting.Value;
            JpegCompressionLevelSetting.Value = settings.JpegCompressionLevelSetting.Value;
            AutoStartWebServerSetting.Value = settings.AutoStartWebServerSetting.Value;
        }
    }
}
