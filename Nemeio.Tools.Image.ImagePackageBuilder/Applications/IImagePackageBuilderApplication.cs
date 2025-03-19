using Nemeio.Tools.Core.Applications;

namespace Nemeio.Tools.Image.ImagePackageBuilder.Applications
{
    internal interface IImagePackageBuilderApplication : IApplication
    {
        public ApplicationStartupSettings Settings { get; set;  }
    }
}
