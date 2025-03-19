using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Images.Jpeg.Builder;
using Nemeio.Core.Images.Renderer;
using Nemeio.Core.Injection;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services;
using Nemeio.LayoutGen.Factory;
using Nemeio.LayoutGen.Renderers;
using Nemeio.LayoutGen.Renderers.Strategies;

//  Needed for unit tests
[assembly: InternalsVisibleTo("Nemeio.LayoutGen.Test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Nemeio.LayoutGen
{
    public sealed class Registerer
    {
        public static void Register(IDependencyRegister register)
        {
            var loggerFactory = register.Resolve<ILoggerFactory>();
            var document = register.Resolve<IDocument>();
            var fontProvider = register.Resolve<IFontProvider>();
            var jpegImagePackageBuilder = register.Resolve<IJpegImagePackageBuilder>();

            var jpegRendererStrategy = CreateJpegRenderer(loggerFactory, document, fontProvider, jpegImagePackageBuilder);
            var oneBppRendererStrategy = CreateOneBppRenderer(loggerFactory, document, fontProvider);

            register.RegisterSingleton(jpegRendererStrategy);
            register.RegisterSingleton(oneBppRendererStrategy);
        }

        public static IJpegRenderer CreateJpegRenderer(ILoggerFactory loggerFactory, IDocument document, IFontProvider fontProvider, IJpegImagePackageBuilder jpegImagePackageBuilder)
        {
            var layoutRenderer = new LayoutRenderer(fontProvider);
            var jpegRendererStrategy = new JpegRendererStrategy(loggerFactory, layoutRenderer, new LGLayoutConverterFactory(document), jpegImagePackageBuilder);

            return jpegRendererStrategy;
        }

        public static IOneBppRenderer CreateOneBppRenderer(ILoggerFactory loggerFactory, IDocument document, IFontProvider fontProvider)
        {
            var layoutRenderer = new LayoutRenderer(fontProvider);
            var oneBppRendererStrategy = new OneBppRendererStrategy(loggerFactory, layoutRenderer, new LGLayoutConverterFactory(document));

            return oneBppRendererStrategy;
        }
    }
}
