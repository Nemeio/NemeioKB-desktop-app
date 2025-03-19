using System;
using System.IO;
using SkiaSharp;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

namespace Nemeio.LayoutGen.Models.Loader
{
    internal class LGSvgLoader : ILGImageLoader
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

            var svg = new SKSvg(new SKSize(size.Width, size.Height));
            svg.Load(filePath);

            return Resize(svg, size);
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

            var svg = new SKSvg(new SKSize(size.Width, size.Height));
            svg.Load(stream);

            return Resize(svg, size);
        }

        private SKBitmap Resize(SKSvg svg, LGSize size)
        {
            var bitmap = new SKBitmap((int)size.Width, (int)size.Height);
            bitmap.Erase(SKColors.Transparent);

            using (var paint = new SKPaint())
            {
                paint.ColorFilter = SKColorFilter.CreateBlendMode(SKColors.Black, SKBlendMode.SrcIn);

                var canvas = new SKCanvas(bitmap);
                canvas.Clear(SKColors.Transparent);
                canvas.DrawPicture(svg.Picture, paint);

                return bitmap;
            }
        }
    }
}
