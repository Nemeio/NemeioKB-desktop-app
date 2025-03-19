using System;
using Nemeio.Core.Images.Renderer;
using Nemeio.Core.Keyboard.Map;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Images.Builders
{
    public abstract class ImageBuilder<T> : IImageBuilder, IImageBuilder<T> where T : ImageFormat
    {
        public T Format { get; private set; }
        public IRenderer<T> Renderer { get; private set; }
        ImageFormat IImageBuilder.Format => Format;

        public ImageBuilder(T format, IRenderer<T> renderer)
        {
            Format = format ?? throw new ArgumentNullException(nameof(format));
            Renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        }

        public byte[] Render(LayoutRenderInfo info, KeyboardMap map)
        {
            var result = Renderer.Render(info, map, Format);

            return result;
        }
    }
}
