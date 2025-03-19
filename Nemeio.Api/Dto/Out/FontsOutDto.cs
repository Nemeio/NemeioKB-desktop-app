using System.Collections.Generic;
using Nemeio.Api.Dto.JsonKeys;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.Out
{
    public class FontsOutDto : BaseOutDto
    {
        [JsonProperty(FontsJsonKeys.Fonts)]
        public IEnumerable<string> Fonts { get; set; }
    }
}
