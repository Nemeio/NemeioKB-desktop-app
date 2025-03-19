using Nemeio.Tools.Core.Applications;

namespace Nemeio.Tools.Firmware.PackageBuilder.Applications
{
    internal interface IPackageBuilderApplication : IApplication
    {
        ApplicationStartupSettings Settings { get; set; }
    }
}
