using Microsoft.Extensions.Logging;

namespace Nemeio.Core.Settings.Types
{
    public sealed class JpegCompressionLevelSetting : NullableIntegerSetting
    {
        public JpegCompressionLevelSetting(ILogger logger, int? value) 
            : base(logger, value) { }

        public override bool IsValid(int? value)
        {
            if (value == null)
            {
                return true;
            }
            else
            {
                return value >= 0 && value <= 90;
            }
        }
    }
}
