using Nemeio.Api.Dto.JsonKeys;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Models.Fonts;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.In.Layout
{
    public class PutFontInDto
    {
        /// <summary>
        /// Font's name. Must be available on application's font.
        /// </summary>
        [JsonProperty(FontJsonKeys.Name, Required = Required.Always)]
        public string Name { get; set; }

        /// <summary>
        /// Three different font size : Small, Medium, Large
        /// </summary>
        [JsonProperty(FontJsonKeys.Size, Required = Required.Always)]
        public FontSize Size { get; set; }

        [JsonProperty(FontJsonKeys.Bold, Required = Required.Always)]
        public bool Bold { get; set; }

        [JsonProperty(FontJsonKeys.Italic, Required = Required.Always)]
        public bool Italic { get; set; }

        public Font ToDomainModel() => new Font(Name, Size, Bold, Italic);
    }
}
