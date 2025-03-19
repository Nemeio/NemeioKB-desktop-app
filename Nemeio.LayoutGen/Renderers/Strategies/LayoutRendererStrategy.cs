using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Enums;
using Nemeio.Core.Images;
using Nemeio.Core.Images.Renderer;
using Nemeio.Core.Keyboard.Map;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Services.Layouts;
using Nemeio.LayoutGen.Extensions;
using Nemeio.LayoutGen.Factory;
using Nemeio.LayoutGen.Models;
using SkiaSharp;

namespace Nemeio.LayoutGen.Renderers.Strategies
{
    internal abstract class LayoutRendererStrategy<T> : IRenderer<T> where T : ImageFormat
    {
        protected readonly LayoutRenderer _renderer;
        protected readonly LGLayoutConverterFactory _layoutConverterFactory;
        protected readonly ILogger _logger;

        protected LayoutRendererStrategy(ILoggerFactory loggerFactory, LayoutRenderer renderer, LGLayoutConverterFactory layoutConverterFactory)
        {
            _logger = loggerFactory.CreateLogger<LayoutRendererStrategy<T>>();
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            _layoutConverterFactory = layoutConverterFactory ?? throw new ArgumentNullException(nameof(layoutConverterFactory));
        }

        public abstract byte[] Render(SKBitmap bitmap, T imageFormat);

        public virtual byte[] Render(LayoutRenderInfo renderInfo, KeyboardMap map, T imageFormat)
        {
            var nemeioMap = new NemeioMap(map);
            var layoutImages = new List<byte>();
            var supportedModifiers = GetModifiersByImageType(renderInfo.ImageType);
            var layoutConverter = _layoutConverterFactory.CreateLayoutConverter(renderInfo.ImageType);

            foreach (var modifier in supportedModifiers)
            {
                var renderLayout = layoutConverter.Convert(modifier, renderInfo.Keys, renderInfo.MainFont, renderInfo.IsDark, nemeioMap, renderInfo.Adjustment);
                using (var image = _renderer.RenderImage(renderLayout))
                {
                    var flippedImage = FlipVerticalImageIfNeeded(image, map, renderInfo);
                    var imageData = Render(flippedImage, imageFormat);

                    layoutImages.AddRange(imageData);
                }
            }

            return layoutImages.ToArray();
        }

        protected SKBitmap FlipVerticalImageIfNeeded(SKBitmap bitmap, KeyboardMap map, LayoutRenderInfo renderInfo)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            if (renderInfo == null)
            {
                throw new ArgumentNullException(nameof(renderInfo));
            }

            if (map.FlipHorizontal)
            {
                var flippedImage = bitmap.FlipHorizontal();

                return flippedImage;
            }

            return bitmap;
        }

        protected IList<KeyboardModifier> GetModifiersByImageType(LayoutImageType imageType)
        {
            if (imageType == LayoutImageType.Classic)
            {
                return new List<KeyboardModifier>() { KeyboardModifier.None };
            }

            return new List<KeyboardModifier>()
            {
                KeyboardModifier.None,
                KeyboardModifier.Capslock,
                KeyboardModifier.Shift,
                KeyboardModifier.AltGr,
                KeyboardModifier.Shift | KeyboardModifier.AltGr,
                KeyboardModifier.Function,
            };
        }
    }
}
