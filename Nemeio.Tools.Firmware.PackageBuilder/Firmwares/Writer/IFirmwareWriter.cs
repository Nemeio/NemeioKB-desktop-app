using System.Threading.Tasks;
using Nemeio.Core.PackageUpdater.Firmware;

namespace Nemeio.Tools.Firmware.PackageBuilder.Firmwares.Writer
{
    internal interface IFirmwareWriter
    {
        Task WriteOnDisk(IPackageFirmware package, string outputFilePath);
    }
}
