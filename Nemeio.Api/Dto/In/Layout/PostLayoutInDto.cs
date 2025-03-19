using Nemeio.Api.Dto.JsonKeys;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.In.Layout
{
    public class PostLayoutInDto
    {
        /// <summary>
        /// Related template's id.
        /// Custom layout will have same information as choosen template.
        /// </summary>
        [JsonProperty(LayoutJsonKeys.TemplateId, Required = Required.Always)]
        public string TemplateId { get; set; }

        /// <summary>
        /// Layout's name. Must be unique.
        /// </summary>
        [JsonProperty(LayoutJsonKeys.Title, Required = Required.Always)]
        public string Title { get; set; }
    }
}
