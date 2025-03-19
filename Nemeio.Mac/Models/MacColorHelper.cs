using AppKit;

namespace Nemeio.Mac.Models
{
    public static class MacColorHelper
    {
        public static NSColor FromHex(int hexValue)
        {
            return NSColor.FromRgb(
                (((float)((hexValue & 0xFF0000) >> 16)) / 255.0f),
                (((float)((hexValue & 0xFF00) >> 8)) / 255.0f),
                (((float)(hexValue & 0xFF)) / 255.0f)
            );
        }
    }
}
