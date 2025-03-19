using System;
using System.IO;
using SkiaSharp;

namespace Nemeio.LayoutGen.Models.Loader
{
    public class LGPngLoader : ILGImageLoader
    {
        public SKBitmap LoadImage(string filePath, LGSize size)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new InvalidDataException($"{nameof(filePath)} is null or empty or whitespace");
            }

            if (size == null)
            {
                throw new ArgumentNullException(nameof(size));
            }

            if (size == LGSize.Zero)
            {
                return null;
            }

            SKBitmap bmp = SKBitmap.Decode(filePath);

            return ResizeImageIfNeeded(bmp, size);
        }

        public SKBitmap LoadImage(Stream stream, LGSize size)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (size == null)
            {
                throw new ArgumentNullException(nameof(size));
            }

            if (size == LGSize.Zero)
            {
                return null;
            }

            SKBitmap bmp = SKBitmap.Decode(stream);

            return ResizeImageIfNeeded(bmp, size);
        }

        private SKBitmap ResizeImageIfNeeded(SKBitmap bmp, LGSize size)
        {
            if (bmp.Width == size.Width && bmp.Height == size.Height)
            {
                return bmp;
            }

            var scaled = bmp.Resize(
                new SKImageInfo((int)size.Width, (int)size.Height),
                SKFilterQuality.High
            );

            return scaled;
        }
    }
}
