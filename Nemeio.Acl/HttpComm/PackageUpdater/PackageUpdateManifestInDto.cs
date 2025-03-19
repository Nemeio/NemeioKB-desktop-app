using Newtonsoft.Json;

namespace Nemeio.Acl.HttpComm.PackageUpdater
{
    internal class PackageUpdateManifestInDto
    {
        [JsonProperty("osx")]
        public SoftwaresPackageUpdateInDto MacOS { get; set; }

        [JsonProperty("win")]
        public SoftwaresPackageUpdateInDto Windows { get; set; }
    }
}
