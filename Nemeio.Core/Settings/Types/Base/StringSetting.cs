using Microsoft.Extensions.Logging;

namespace Nemeio.Core.Settings
{
    public abstract class StringSetting : Setting<string>
    {
        protected StringSetting(ILogger logger, string value) 
            : base(logger, value) { }
    }
}
