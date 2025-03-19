using System;
using Newtonsoft.Json;

namespace Nemeio.Tools.Firmware.PackageBuilder.Files.Manifest
{
    public class FirmwarePackageItemDto
    {
        [JsonProperty("version")]
        public Version Version { get; set; }

        [JsonProperty("name")]
        public string Filename { get; set; }
    }
}
