using System;
using Newtonsoft.Json;

namespace Nemeio.Tools.Testing.Update.Application.Update.Tester
{
    internal class TestSettingsDto
    {
        [JsonProperty("versionRange")]
        public UpdateVersionRangeInDto VersionRange { get; set; }

        [JsonProperty("environmentName")]
        public string EnvironmentName { get; set; }

        [JsonProperty("serverUri")]
        public Uri ServerUri { get; set; }

        [JsonProperty("outputPath")]
        public string OutputPath { get; set; }
    }
}
