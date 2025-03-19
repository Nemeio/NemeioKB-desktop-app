using System.Threading.Tasks;

namespace Nemeio.Tools.Firmware.PackageBuilder.Files.Manifest
{
    public interface IFirmwareManifestReader
    {
        Task<FirmwareManifest> ReadAsync(string path);
    }
}
