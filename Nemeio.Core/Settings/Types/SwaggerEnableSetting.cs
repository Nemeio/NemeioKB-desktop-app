using Microsoft.Extensions.Logging;

namespace Nemeio.Core.Settings.Types
{
    public sealed class SwaggerEnableSetting : NullableBooleanSetting
    {
        public SwaggerEnableSetting(ILogger logger, bool? value) 
            : base(logger, value) { }
    }
}
