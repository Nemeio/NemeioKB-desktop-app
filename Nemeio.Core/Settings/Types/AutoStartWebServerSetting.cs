using Microsoft.Extensions.Logging;

namespace Nemeio.Core.Settings.Types
{
    public sealed class AutoStartWebServerSetting : NullableBooleanSetting
    {
        public AutoStartWebServerSetting(ILogger logger, bool? value) 
            : base(logger, value) { }
    }
}
