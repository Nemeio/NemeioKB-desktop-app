using Microsoft.Extensions.DependencyInjection;
using Nemeio.Core.FileSystem;
using Nemeio.Tools.Firmware.PackageBuilder.Applications;
using Nemeio.Tools.Firmware.PackageBuilder.Builders;
using Nemeio.Tools.Firmware.PackageBuilder.Files;
using Nemeio.Tools.Firmware.PackageBuilder.Files.Manifest;
using Nemeio.Tools.Firmware.PackageBuilder.Firmwares.Composer;
using Nemeio.Tools.Firmware.PackageBuilder.Firmwares.Writer;

namespace Nemeio.Tools.Firmware.PackageBuilder
{
    class Program
    {
        static int Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IFileSystem, FileSystem>();
            serviceCollection.AddTransient<IInputFileFactory, InputFileFactory>();
            serviceCollection.AddTransient<INrfManifestReader, NrfManifestReader>();
            serviceCollection.AddTransient<IFirmwareManifestReader, FirmwareManifestReader>();
            serviceCollection.AddTransient<IPackageFirmwareBuilder, PackageFirmwareBuilder>();
            serviceCollection.AddTransient<IFirmwareWriter, FirmwareWriter>();
            serviceCollection.AddTransient<IUpdateInformationComposer, UpdateInformationComposer>();
            serviceCollection.AddSingleton<IPackageBuilderApplication, Application>();  

             var ioc = serviceCollection.BuildServiceProvider();

            var consoleApplication = new ConsoleApplication();

            return consoleApplication.Run(ioc, args);
        }
    }
}
