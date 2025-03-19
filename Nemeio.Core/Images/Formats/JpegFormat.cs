using Nemeio.Core.Settings.Types;

namespace Nemeio.Core.Images.Formats
{
    public sealed class JpegFormat : ImageFormat
    {
        public const int DefaultJpegCompressionLevel = 80;

        private readonly JpegCompressionLevelSetting _setting;

        public int CompressionLevel => _setting?.Value ?? DefaultJpegCompressionLevel;

        public JpegFormat(JpegCompressionLevelSetting setting)
            : base(255) 
        {
            _setting = setting;
        }
    }
}
