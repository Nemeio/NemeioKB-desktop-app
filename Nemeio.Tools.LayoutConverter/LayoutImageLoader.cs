using System;
using SkiaSharp;

namespace Nemeio.Tools.LayoutConverter
{
    internal class LayoutImageLoader
    {
        internal SKBitmap LoadImage(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            return SKBitmap.Decode(filePath);
        }
    }
}
