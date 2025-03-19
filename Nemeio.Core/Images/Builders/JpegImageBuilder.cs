using Nemeio.Core.Images.Formats;
using Nemeio.Core.Images.Renderer;

namespace Nemeio.Core.Images.Builders
{
    public sealed class JpegImageBuilder : ImageBuilder<JpegFormat>
    {
        public JpegImageBuilder(JpegFormat format, IRenderer<JpegFormat> renderer) 
            : base(format, renderer) { }
    }
}
