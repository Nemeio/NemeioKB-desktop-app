using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nemeio.Acl.HttpComm.PackageUpdater
{
    internal class SoftwaresPackageUpdateInDto
    {
        [JsonProperty("softwares")]
        public IList<SoftwarePackageUpdateInDto> Softwares { get; set; }
    }
}
