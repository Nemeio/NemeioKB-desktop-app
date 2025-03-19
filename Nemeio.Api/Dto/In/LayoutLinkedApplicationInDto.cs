using System.Collections.Generic;
using Nemeio.Api.Dto.JsonKeys;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.In
{
    public class LayoutLinkedApplicationInDto
    {
        [JsonProperty(LinkedApplicationJsonKeys.ApplicationPath, Required = Required.Always)]
        public IEnumerable<string> AppPath { get; set; }

        [JsonProperty(LinkedApplicationJsonKeys.Enable, Required = Required.Always)]
        public bool Enable { get; set; }
    }
}
