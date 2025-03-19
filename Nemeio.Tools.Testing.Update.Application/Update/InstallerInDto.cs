using System;
using Nemeio.Tools.Testing.Update.Core.Update.Installer;
using Newtonsoft.Json;

namespace Nemeio.Tools.Testing.Update.Application.Update
{
    public class InstallerInDto
    {
        [JsonProperty("version")]
        public Version Version { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        public Installer ToModel() => new Installer(Url, Version);
    }
}
