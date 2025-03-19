using Nemeio.Api.Dto.JsonKeys;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.In.Layout
{
    public class PutLayoutSubactionInDto
    {
        /// <summary>
        /// Data linked to the current subaction. Depends of current type.
        /// </summary>
        [JsonProperty(SubactionJsonKeys.Data, Required = Required.Always)]
        public string Data { get; set; }

        /// <summary>
        /// Subaction type. Can be :
        /// Unicode = 1, Special = 2, Application = 3, Url = 4, Layout = 5, Back = 7, Forward = 8
        /// </summary>
        [JsonProperty(SubactionJsonKeys.Type, Required = Required.Always)]
        public KeyActionType Type { get; set; }

        public KeySubAction ToDomainModel() => new KeySubAction(Data, Type);
    }
}
