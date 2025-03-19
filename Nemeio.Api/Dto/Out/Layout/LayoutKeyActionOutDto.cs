using System.Collections.Generic;
using System.Linq;
using Nemeio.Api.Dto.JsonKeys;
using Nemeio.Core.DataModels.Configurator;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.Out.Layout
{
    public enum KeyModifierOutDto
    {
        None = 0,
        Shift = 1,
        AltGr = 2,
        Both = 3,
        Function = 4
    }

    public class LayoutKeyActionOutDto : BaseOutDto
    {
        [JsonProperty(ActionJsonKeys.Display)]
        public string Display { get; set; }

        [JsonProperty(ActionJsonKeys.Modifier)]
        public KeyModifierOutDto Modifier { get; set; }
        
        [JsonProperty(ActionJsonKeys.IsGrey)]
        public bool IsGrey { get; set; }

        [JsonProperty(ActionJsonKeys.Subactions)]
        public IEnumerable<LayoutSubactionOutDto> Subactions { get; set; }

        public static LayoutKeyActionOutDto FromModel(KeyAction act) => new LayoutKeyActionOutDto()
        {
            Display = act.Display,
            Modifier = (KeyModifierOutDto)act.Modifier,
            IsGrey = act.IsGrey,
            Subactions = act.Subactions != null ? act.Subactions.Select(x => LayoutSubactionOutDto.FromModel(x)) : new List<LayoutSubactionOutDto>()
        };
    }
}
