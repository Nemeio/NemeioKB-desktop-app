using Newtonsoft.Json;

namespace Nemeio.Api.Dto.In.Layout
{
    public class DuplicateLayoutInDto
    {
        /// <summary>
        /// Duplicated layout id
        /// </summary>
        [JsonProperty("layoutId", Required = Required.Always)]
        public string LayoutId { get; set; }

        /// <summary>
        /// Duplicated layout new name. Must be unique.
        /// </summary>
        [JsonProperty("title", Required = Required.Default)]
        public string Title { get; set; }
    }
}
