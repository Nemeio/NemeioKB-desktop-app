using System.Threading.Tasks;
using Nemeio.Core.PackageUpdater.Firmware;

namespace Nemeio.Tools.Firmware.PackageBuilder.Builders
{
    internal interface IPackageFirmwareBuilder
    {
        Task<IPackageFirmware> CreatePackageAsync(FirmwareUpdateInformation manifest);
    }
}
