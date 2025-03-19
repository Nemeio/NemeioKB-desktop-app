using Microsoft.Extensions.Logging;

namespace Nemeio.Core.Settings
{
    public abstract class NullableIntegerSetting : Setting<int?>
    {
        protected NullableIntegerSetting(ILogger logger, int? value) 
            : base(logger, value) { }
    }
}
