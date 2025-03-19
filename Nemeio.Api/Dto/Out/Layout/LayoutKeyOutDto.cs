using System.Collections.Generic;
using System.Linq;
using Nemeio.Api.Dto.JsonKeys;
using Nemeio.Core.DataModels.Configurator;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.Out.Layout
{
    public enum LayoutApiOutDto_Key_BackgroundMode
    {
        Light   = 0,
        Dark    = 1
    }

    public class LayoutKeyOutDto : BaseOutDto
    {
        [JsonProperty(KeyJsonKeys.Index)]
        public int Index { get; set; }

        [JsonProperty(KeyJsonKeys.Disposition)]
        public string Disposition { get; set; }

        [JsonProperty(KeyJsonKeys.Actions)]
        public IList<LayoutKeyActionOutDto> Actions { get; set; }

        [JsonProperty(KeyJsonKeys.Font)]
        public LayoutKeyFontOutDto Font { get; set; }

        [JsonProperty(KeyJsonKeys.Edited)]
        public bool Edited { get; set; }

        public static LayoutKeyOutDto FromModel(Key key) => new LayoutKeyOutDto()
        {
            Index = key.Index,
            Disposition = key.Disposition.ToString(),
            Actions = key.Actions.Select(x => LayoutKeyActionOutDto.FromModel(x)).ToList(),
            Font = LayoutKeyFontOutDto.FromModel(key.Font),
            Edited = key.Edited
        };
    }
}
