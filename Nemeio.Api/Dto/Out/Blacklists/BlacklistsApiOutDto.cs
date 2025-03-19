using System.Collections.Generic;
using Nemeio.Api.Dto.JsonKeys;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.Out.Blacklists
{
    public class BlacklistsApiOutDto : BaseOutDto
    {
        [JsonProperty(BlacklistJsonKeys.Blacklists)]
        public IList<BlacklistApiOutDto> Blacklists { get; set; }
    }
}
