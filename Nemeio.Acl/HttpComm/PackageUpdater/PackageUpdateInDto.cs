using System;
using Newtonsoft.Json;

namespace Nemeio.Acl.HttpComm.PackageUpdater
{
    internal class PackageUpdateInDto
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("checksum")]
        public string Checksum { get; set; }

        [JsonProperty("version")]
        public Version Version { get; set; }
    }
}
