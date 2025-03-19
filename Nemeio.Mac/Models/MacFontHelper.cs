using AppKit;

namespace Nemeio.Mac.Models
{
    public static class MacFontHelper
    {
        private const string OpenSansRegularFontName    = "OpenSans-Regular";
        private const string OpenSansBoldFontName       = "OpenSans-Bold";

        public static NSFont GetOpenSans(float forSize)
        {
            return NSFont.FromFontName(OpenSansRegularFontName, forSize);
        }

        public static NSFont GetOpenSansBold(float forSize)
        {
            return NSFont.FromFontName(OpenSansBoldFontName, forSize);
        }
    }
}
