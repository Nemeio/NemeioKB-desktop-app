using System.Collections.Generic;
using Nemeio.Api.Dto.JsonKeys;
using Nemeio.Core.Enums;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.In.Layout
{
    public class PutLayoutKeyActionInDto
    {
        /// <summary>
        /// Value displayed on keyboard.
        /// UI settings.
        /// </summary>
        [JsonProperty(ActionJsonKeys.Display, Required = Required.Default)]
        public string Display { get; set; }

        /// <summary>
        /// Modifier linked to this action. A key can only have one time each modifier.
        /// </summary>
        [JsonProperty(ActionJsonKeys.Modifier, Required = Required.Always)]
        public KeyboardModifier Modifier { get; set; }
        
        /// <summary>
        /// To let API manage grey level depending on White/Black background #
        /// </summary>
        [JsonProperty(ActionJsonKeys.IsGrey, Required = Required.Default)]
        public bool IsGrey { get; set; }

        [JsonProperty(ActionJsonKeys.Subactions, Required = Required.Default)]
        public IList<PutLayoutSubactionInDto> Subactions { get; set; }
    }
}
