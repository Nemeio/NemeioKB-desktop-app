using Nemeio.Api.Dto.JsonKeys;
using Nemeio.Core;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Models.Fonts;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.Out.Layout
{
    public class LayoutKeyFontOutDto : BaseOutDto
    {
        [JsonProperty(FontJsonKeys.Name)]
        public string Name { get; set; }

        [JsonProperty(FontJsonKeys.Size)]
        public FontSize Size { get; set; }

        [JsonProperty(FontJsonKeys.Bold)]
        public bool Bold { get; set; }

        [JsonProperty(FontJsonKeys.Italic)]
        public bool Italic { get; set; }

        public static LayoutKeyFontOutDto FromModel(Font fnt) => new LayoutKeyFontOutDto()
        {
            Bold = fnt == null ? false : fnt.Bold,
            Italic = fnt == null ? false : fnt.Italic,
            Name = fnt == null ? NemeioConstants.Noto : fnt.Name,
            Size = fnt == null ? FontSize.Medium : fnt.Size
        };
    }
}
