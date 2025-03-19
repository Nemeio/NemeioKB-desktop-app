using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

//  Needed for unit tests
[assembly: InternalsVisibleTo("Nemeio.Tools.LayoutConverter.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Nemeio.Tools.LayoutConverter.Validators
{
    internal enum ImageFormat
    {
        OneBitPerPixel,
        TwoBitsPerPixel
    }

    internal class ImageFormatValidator
    {
        public const string OneBitPerPixel = "1bpp";
        public const string TwoBitPerPixel = "2bpp";

        internal static IEnumerable<string> SupportedFormats = new List<string>()
        {
            OneBitPerPixel,
            TwoBitPerPixel
        };

        internal ImageFormatValidator() { }

        internal bool IsValidFormat(string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                return false;
            }

            return SupportedFormats.Any(x => x.Equals(format.ToLower()));
        }

        internal ImageFormat GetFormatByName(string formatName)
        {
            switch (formatName.ToLower())
            {
                case OneBitPerPixel:
                    return ImageFormat.OneBitPerPixel;
                case TwoBitPerPixel:
                    return ImageFormat.TwoBitsPerPixel;
                default:
                    throw new InvalidOperationException($"Format <{formatName}> is not supported!");
            }
        }
    }
}
