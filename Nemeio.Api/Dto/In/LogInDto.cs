using Newtonsoft.Json;

namespace Nemeio.Api.Dto.In
{
    public class LogInDto
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
