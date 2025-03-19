using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nemeio.Tools.Testing.Update.Application.Update
{
    public class InstallersInDto
    {
        [JsonProperty("softwares")]
        public IEnumerable<InstallerInDto> Installers { get; set; }
    }
}
