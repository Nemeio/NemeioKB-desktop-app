using Nemeio.Tools.Core.Files;
using Nemeio.Tools.Firmware.PackageBuilder.Builders;

namespace Nemeio.Tools.Firmware.PackageBuilder.Files
{
    internal interface IInputFileFactory
    {
        InputFile CreateFirmwareInputFile(FirmwareUpdateModule module, string filePath);
        InputFile CreateManifestInputFile(string filePath);
    }
}
