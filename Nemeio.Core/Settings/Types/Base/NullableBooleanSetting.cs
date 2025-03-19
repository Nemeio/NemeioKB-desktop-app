using Microsoft.Extensions.Logging;

namespace Nemeio.Core.Settings
{
    public abstract class NullableBooleanSetting : Setting<bool?>
    {
        protected NullableBooleanSetting(ILogger logger, bool? value) 
            : base(logger, value) { }
    }
}
