using Nemeio.Tools.Firmware.PackageBuilder.Builders;
using Nemeio.Tools.Firmware.PackageBuilder.Files.Manifest;

namespace Nemeio.Tools.Firmware.PackageBuilder.Firmwares.Composer
{
    internal interface IUpdateInformationComposer
    {
        FirmwareUpdateInformation Compose(FirmwareManifest manifest, string fileDirectoryPath);
    }
}
