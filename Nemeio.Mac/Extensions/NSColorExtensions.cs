using AppKit;

namespace Nemeio.Mac.Extensions
{
    public static class NSColorExtensions
    {
        public static NSColor FromHex(this NSColor color, int hexValue)
        {
            return NSColor.FromRgb(
                (((float)((hexValue & 0xFF0000) >> 16)) / 255.0f),
                (((float)((hexValue & 0xFF00) >> 8)) / 255.0f),
                (((float)(hexValue & 0xFF)) / 255.0f)
            );
        }
    }
}
