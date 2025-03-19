using Newtonsoft.Json;

namespace Nemeio.Tools.Firmware.PackageBuilder
{
    public class NrfManifestEntryDto
    {
        [JsonProperty("application")]
        public NrfComponentDto Application { get; set; }

        [JsonProperty("softdevice_bootloader")]
        public NrfComponentDto SoftDevice { get; set; }
    }
}
