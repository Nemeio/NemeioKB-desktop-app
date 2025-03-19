using Nemeio.Core.Images.Formats;
using Nemeio.Core.Images.Renderer;

namespace Nemeio.Core.Images.Builders
{
    public sealed class OneBppImageBuilder : ImageBuilder<OneBppFormat>
    {
        public OneBppImageBuilder(OneBppFormat format, IRenderer<OneBppFormat> renderer) 
            : base(format, renderer) { }
    }
}
