using Newtonsoft.Json;

namespace Nemeio.Api.Dto.Out
{
    public class AugmentedImageTypeAvailabilityOutDto
    {
        [JsonProperty("classic")]
        public bool Classic { get; set; }

        [JsonProperty("hide")]
        public bool Hide { get; set; }

        [JsonProperty("gray")]
        public bool Gray { get; set; }

        [JsonProperty("bold")]
        public bool Bold { get; set; }
    }
}
