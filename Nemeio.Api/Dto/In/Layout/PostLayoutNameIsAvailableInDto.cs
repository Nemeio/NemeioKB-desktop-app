using Newtonsoft.Json;

namespace Nemeio.Api.Dto.In.Layout
{
    public class PostLayoutNameIsAvailableInDto
    {
        /// <summary>
        /// Search name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
