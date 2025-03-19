using Microsoft.Extensions.Logging;
using Nemeio.Core.Images.Formats;
using Nemeio.LayoutGen.Extensions;
using Nemeio.LayoutGen.Factory;
using SkiaSharp;

namespace Nemeio.LayoutGen.Renderers.Strategies
{
    internal sealed class PngRendererStrategy : LayoutRendererStrategy<PngFormat>
    {
        public PngRendererStrategy(ILoggerFactory loggerFactory, LayoutRenderer renderer, LGLayoutConverterFactory layoutConverterFactory) 
            : base(loggerFactory, renderer, layoutConverterFactory) { }

        public override byte[] Render(SKBitmap bitmap, PngFormat imageFormat)
        {
            var png = bitmap
                .ToPng()
                .ToArray();

            return png;
        }
    }
}
