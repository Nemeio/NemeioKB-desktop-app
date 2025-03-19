using Newtonsoft.Json;

namespace Nemeio.Presentation.PackageUpdater.Firmware
{
    public class FirmwarePackageManifestDto
    {
        [JsonProperty("cpu")]
        public FirmwarePackageItemDto Cpu { get; set; }

        [JsonProperty("ble")]
        public FirmwarePackageItemDto BluetootLE { get; set; }
        
        [JsonProperty("ite")]
        public FirmwarePackageItemDto Ite { get; set; } 
    }
}
