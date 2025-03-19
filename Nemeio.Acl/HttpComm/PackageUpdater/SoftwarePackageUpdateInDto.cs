using Newtonsoft.Json;

namespace Nemeio.Acl.HttpComm.PackageUpdater
{
    internal enum PlatformTypeInDto
    {
        x64,
        x86
    }

    internal class SoftwarePackageUpdateInDto : PackageUpdateInDto
    {
        [JsonProperty("platform")]
        public PlatformTypeInDto Platform { get; set; }
    }
}
