using System.Threading.Tasks;

namespace Nemeio.Tools.Firmware.PackageBuilder
{
    public interface INrfManifestReader
    {
        Task<NrfManifest> ParseManifest(string manifestFilePath);
    }
}
