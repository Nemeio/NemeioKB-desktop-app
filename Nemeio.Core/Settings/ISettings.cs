using Nemeio.Core.Settings.Types;

namespace Nemeio.Core.Settings
{
    public interface ISettings
    {
        SwaggerEnableSetting SwaggerEnable { get; }
        ApiPortSetting ApiPort { get; }
        EnvironmentSetting EnvironmentSetting { get; }
        JpegCompressionLevelSetting JpegCompressionLevelSetting { get; }
        AutoStartWebServerSetting AutoStartWebServerSetting { get; }

        void Update(ISettings settings);
    }
}
