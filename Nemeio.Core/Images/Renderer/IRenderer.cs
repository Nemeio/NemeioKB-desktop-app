using Nemeio.Core.Keyboard.Map;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Images.Renderer
{
    public interface IRenderer<T> where T : ImageFormat
    {
        byte[] Render(LayoutRenderInfo renderInfo, KeyboardMap map, T imageFormat);
    }
}
