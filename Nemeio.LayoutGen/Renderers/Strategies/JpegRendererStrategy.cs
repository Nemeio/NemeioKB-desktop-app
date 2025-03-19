using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Images.Formats;
using Nemeio.Core.Images.Jpeg;
using Nemeio.Core.Images.Jpeg.Builder;
using Nemeio.Core.Images.Renderer;
using Nemeio.Core.Keyboard.Map;
using Nemeio.Core.PackageUpdater.Firmware;
using Nemeio.Core.Services.Layouts;
using Nemeio.LayoutGen.Factory;
using Nemeio.LayoutGen.Models;
using SkiaSharp;

namespace Nemeio.LayoutGen.Renderers.Strategies
{
    internal sealed class JpegRendererStrategy : LayoutRendererStrategy<JpegFormat>, IJpegRenderer
    {
        private readonly IJpegImagePackageBuilder _packageBuilder;

        public JpegRendererStrategy(ILoggerFactory loggerFactory, LayoutRenderer renderer, LGLayoutConverterFactory layoutConverterFactory, IJpegImagePackageBuilder packageBuilder) 
            : base(loggerFactory, renderer, layoutConverterFactory) 
        {
            _packageBuilder = packageBuilder ?? throw new ArgumentNullException(nameof(packageBuilder));
        }

        public override byte[] Render(SKBitmap bitmap, JpegFormat imageFormat)
        {
            _logger.LogInformation($"Render image with Jpeg format and compression level <{imageFormat.CompressionLevel}>");

            using (var jpg = SKImage.FromBitmap(bitmap))
            using (var image = jpg.Encode(SKEncodedImageFormat.Jpeg, imageFormat.CompressionLevel))
            {
                var data = image.ToArray();

                return data;
            }
        }

        public override byte[] Render(LayoutRenderInfo renderInfo, KeyboardMap map, JpegFormat imageFormat)
        {
            var nemeioMap = new NemeioMap(map);
            var imageData = new List<JpegImageData>();
            var supportedModifiers = GetModifiersByImageType(renderInfo.ImageType);
            var layoutConverter = _layoutConverterFactory.CreateLayoutConverter(renderInfo.ImageType);

            foreach (var modifier in supportedModifiers)
            {
                var renderLayout = layoutConverter.Convert(modifier, renderInfo.Keys, renderInfo.MainFont, renderInfo.IsDark, nemeioMap, renderInfo.Adjustment);
                using (var image = _renderer.RenderImage(renderLayout))
                {
                    var flippedImage = FlipVerticalImageIfNeeded(image, map, renderInfo);
                    var data = Render(flippedImage, imageFormat);

                    //  FIXME [KSB] : Hardcoded value until we allow user to change that
                    var newImageData = new JpegImageData(data, PackageCompressionType.None);

                    imageData.Add(newImageData);
                }
            }

            var package = _packageBuilder.CreatePackage(imageData);

            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream))
            {
                package.Convert(writer);

                return memoryStream.ToArray();
            }
        }
    }
}
