using System;
using Newtonsoft.Json;

namespace Nemeio.Api.JsonConverters
{
    internal class OptionalJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var genericAccess = value as IGenericAccess;
            if (genericAccess == null)
            {
                throw new InvalidOperationException("OptionalJsonConverter used on non-optional property");
            }

            if (!genericAccess.IsValuePresent)
            {
                writer.WriteUndefined();
                return;
            }

            serializer.Serialize(writer, genericAccess.Value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (existingValue == null)
            {
                existingValue = Activator.CreateInstance(objectType);
            }

            if (reader.TokenType != JsonToken.Undefined)
            {
                var genericAccess = existingValue as IGenericAccess;
                genericAccess.Value = serializer.Deserialize(reader, genericAccess.GenericType);
            }

            return existingValue;
        }

        public override bool CanConvert(Type objectType) => true;
    }
}
