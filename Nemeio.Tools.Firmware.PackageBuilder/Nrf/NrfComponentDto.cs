using Newtonsoft.Json;

namespace Nemeio.Tools.Firmware.PackageBuilder
{
    public class NrfComponentDto
    {
        [JsonProperty("bin_file")]
        public string BinFile { get; set; }

        [JsonProperty("dat_file")]
        public string DatFile { get; set; }
    }
}
