using Nemeio.Api.Dto.JsonKeys;
using Nemeio.Core.DataModels.Configurator;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.Out.Layout
{
    public enum KeyActionTypeOutDto
    {
        Unicode = 1,
        Special = 2,
        Application = 3,
        Url = 4,
        Layout = 5
    }

    public class LayoutSubactionOutDto : BaseOutDto
    {
        [JsonProperty(SubactionJsonKeys.Data)]
        public string Data { get; set; }

        [JsonProperty(SubactionJsonKeys.Type)]
        public KeyActionTypeOutDto Type { get; set; }

        public static LayoutSubactionOutDto FromModel(KeySubAction subaction) => new LayoutSubactionOutDto()
        {
            Data = subaction.Data,
            Type = (KeyActionTypeOutDto) subaction.Type
        };
    }
}
