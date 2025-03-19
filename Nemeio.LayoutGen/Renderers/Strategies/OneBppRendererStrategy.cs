using System.IO;
using System.IO.Compression;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Images.Formats;
using Nemeio.Core.Images.Renderer;
using Nemeio.Core.Keyboard.Map;
using Nemeio.Core.Services.Layouts;
using Nemeio.LayoutGen.Extensions;
using Nemeio.LayoutGen.Factory;
using SkiaSharp;

namespace Nemeio.LayoutGen.Renderers.Strategies
{
    internal sealed class OneBppRendererStrategy : LayoutRendererStrategy<OneBppFormat>, IOneBppRenderer
    {
        public OneBppRendererStrategy(ILoggerFactory loggerFactory, LayoutRenderer renderer, LGLayoutConverterFactory layoutConverterFactory) 
            : base(loggerFactory, renderer, layoutConverterFactory) { }

        public override byte[] Render(SKBitmap bitmap, OneBppFormat imageFormat)
        {
            var image = bitmap.ConvertTo1Bpp();

            return image;
        }

        public override byte[] Render(LayoutRenderInfo renderInfo, KeyboardMap map, OneBppFormat imageFormat)
        {
            var images = base.Render(renderInfo, map, imageFormat);

            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionLevel.Optimal))
            {
                zipStream.Write(images, 0, images.Length);
                zipStream.Close();

                return compressedStream.ToArray();
            }
        }
    }
}
