using System.Collections.Generic;
using Nemeio.Api.Dto.JsonKeys;
using Nemeio.Core.Enums;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.In.Layout
{
    /// <summary>
    /// All key related information to update
    /// </summary>
    public class PutLayoutKeyInDto
    {
        [JsonProperty(KeyJsonKeys.Actions, Required = Required.Always)]
        public IList<PutLayoutKeyActionInDto> Actions { get; set; }

        [JsonProperty(KeyJsonKeys.Disposition, Required = Required.Always)]
        public KeyDisposition Disposition { get; set; }

        [JsonProperty(KeyJsonKeys.Font, Required = Required.Default)]
        public PutFontInDto Font { get; set; }
    }
}
