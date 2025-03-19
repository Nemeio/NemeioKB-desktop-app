using Nemeio.Api.Dto.JsonKeys;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.Out.Templates
{
    public class TemplateOutDto : BaseOutDto
    {
        [JsonProperty(LayoutJsonKeys.Id)]
        public string Id { get; set; }

        [JsonProperty(LayoutJsonKeys.Title)]
        public string Title { get; set; }
    }
}
