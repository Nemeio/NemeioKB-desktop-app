using System.Collections.Generic;
using Nemeio.Api.Dto.JsonKeys;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.Out.Templates
{
    public class TemplatesOutDto : BaseOutDto
    {
        [JsonProperty(TemplatesJsonKeys.Templates)]
        public IEnumerable<TemplateOutDto> Templates { get; set; }
    }
}
