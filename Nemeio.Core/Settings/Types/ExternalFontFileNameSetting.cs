using Microsoft.Extensions.Logging;

namespace Nemeio.Core.Settings.Types
{
    public sealed class ExternalFontFileNameSetting : StringSetting
    {
        public ExternalFontFileNameSetting(ILogger logger, string value)
            : base(logger, value) { }
    }
}