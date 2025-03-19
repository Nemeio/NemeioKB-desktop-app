using System;
using Nemeio.Api.Dto.In.Events;
using Newtonsoft.Json;

namespace Nemeio.Api.Converters
{
    public class KeyboardParameterTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((int)value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value is Int64 enumValue)
            {
                if (Enum.IsDefined(typeof(EventType), (int)enumValue))
                {
                    return Enum.Parse(typeof(EventType), enumValue.ToString());
                }
            }

            throw new InvalidCastException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(int);
        }
    }
}
