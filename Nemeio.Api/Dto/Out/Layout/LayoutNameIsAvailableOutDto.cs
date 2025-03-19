using Newtonsoft.Json;

namespace Nemeio.Api.Dto.Out.Layout
{
    public class LayoutNameIsAvailableOutDto : BaseOutDto
    {
        [JsonProperty("isAvailable")]
        public bool IsAvailable { get; private set; }

        public LayoutNameIsAvailableOutDto(bool isAvailable)
        {
            IsAvailable = isAvailable;
        }
    }
}
