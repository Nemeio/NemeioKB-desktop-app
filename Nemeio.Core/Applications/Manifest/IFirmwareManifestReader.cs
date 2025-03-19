using System.Threading.Tasks;

namespace Nemeio.Core.Applications.Manifest
{
    public interface IFirmwareManifestReader
    {
        Task<FirmwareManifest> ReadAsync(string path);
    }
}
