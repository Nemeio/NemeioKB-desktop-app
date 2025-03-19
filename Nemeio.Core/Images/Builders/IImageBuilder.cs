using Nemeio.Core.Images.Renderer;
using Nemeio.Core.Keyboard.Map;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Images.Builders
{
    public interface IImageBuilder
    {
        ImageFormat Format { get; }
        byte[] Render(LayoutRenderInfo info, KeyboardMap map);
    }

    public interface IImageBuilder<T> : IImageBuilder where T : ImageFormat
    {
        new T Format { get; }
        IRenderer<T> Renderer { get; }
    }
}
