using Microsoft.Extensions.Logging;

namespace Nemeio.Core.Settings.Types
{
    public sealed class EnvironmentSetting : StringSetting
    {
        public EnvironmentSetting(ILogger logger, string value) 
            : base(logger, value) { }
    }
}
