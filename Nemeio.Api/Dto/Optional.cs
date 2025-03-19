using System;
using Nemeio.Api.JsonConverters;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto
{
    [JsonConverter(typeof(OptionalJsonConverter))]
    public struct Optional<T> : IGenericAccess
    {
        private bool _isValuePresent;
        private T _value;

        public bool IsValuePresent
        {
            get => _isValuePresent;
            set
            {
                _isValuePresent = value;
                if (!value)
                    Value = default;
            }
        }

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                _isValuePresent = true;
            }
        }

        public Type GenericType => typeof(T);

        object IGenericAccess.Value 
        {
            set => Value = (T)value;
            get => Value;
        }

        public static implicit operator Optional<T>(T value) => new Optional<T> { Value = value };

        public static explicit operator T(Optional<T> optional)
        {
            if (!optional.IsValuePresent)
            {
                throw new InvalidOperationException("The optional value is not present");
            }

            return optional.Value;
        }

        public override string ToString()
        {
            if (!IsValuePresent)
            {
                return "{ no value }";
            }

            if (Value == null)
            {
                return "{ null }";
            }

            return Value.ToString();
        }
    }
}
