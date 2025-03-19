using Nemeio.Api.Dto.JsonKeys;
using Nemeio.Core.Services.Blacklist;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.Out.Blacklists
{
    public class BlacklistApiOutDto : BaseOutDto
    {
        [JsonProperty(BlacklistJsonKeys.Id)]
        public long Id { get; set; }

        [JsonProperty(BlacklistJsonKeys.Name)]
        public string Name { get; set; }

        [JsonProperty(BlacklistJsonKeys.IsSystem)]
        public bool IsSystem { get; set; }

        public static BlacklistApiOutDto FromModel(Blacklist blacklist) => new BlacklistApiOutDto()
        {
            Id = blacklist.Id,
            Name = blacklist.Name,
            IsSystem = blacklist.IsSystem
        };
    }
}
