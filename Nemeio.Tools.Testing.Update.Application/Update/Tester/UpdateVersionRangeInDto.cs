using System;
using Newtonsoft.Json;

namespace Nemeio.Tools.Testing.Update.Application.Update.Tester
{
    internal class UpdateVersionRangeInDto
    {
        [JsonProperty("start")]
        public Version Start { get; set; }

        [JsonProperty("end")]
        public Version End { get; set; }
    }
}
