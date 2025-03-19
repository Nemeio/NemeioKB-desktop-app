using Newtonsoft.Json;

namespace Nemeio.Api.Dto.In.Events
{
    public class EventInDto<T>
    {
        [JsonProperty("value", Required = Required.Always)]
        public T Value { get; set; }
    }
}
