using Newtonsoft.Json;

namespace Nemeio.Tools.Firmware.PackageBuilder
{
    public class NrfManifestDto
    {
        [JsonProperty("manifest")]
        public NrfManifestEntryDto Manifest { get; set; }
    }
}
