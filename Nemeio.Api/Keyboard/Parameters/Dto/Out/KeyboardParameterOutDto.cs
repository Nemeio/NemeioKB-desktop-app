using Nemeio.Api.Dto.In.Events;
using Newtonsoft.Json;

namespace Nemeio.Api.Keyboard.Parameters.Dto.Out
{
    public sealed class KeyboardParameterOutDto<T>
    {
        [JsonProperty("value")]
        public T Value { get; private set; }

        [JsonProperty("type")]
        public EventType Type { get; private set; }

        public KeyboardParameterOutDto(EventType type, T value)
        {
            Type = type;
            Value = value;
        }
    }
}
