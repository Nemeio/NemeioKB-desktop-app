using System;
using Newtonsoft.Json;

namespace Nemeio.Presentation.PackageUpdater.Firmware
{
    public class FirmwarePackageItemDto
    {
        [JsonProperty("version")]
        public Version Version { get; set; }

        [JsonProperty("name")]
        public string Filename { get; set; }
    }
}
