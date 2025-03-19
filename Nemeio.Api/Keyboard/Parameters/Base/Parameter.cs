using System;
using Nemeio.Api.Dto.In.Events;
using Nemeio.Api.Keyboard.Parameters.Base;
using Nemeio.Core.Keyboard.Parameters;
using Newtonsoft.Json;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal abstract class Parameter<T> : IParameter
    {
        protected readonly KeyboardParameters _parameters;

        protected Parameter(KeyboardParameters parameters)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        public void Update(KeyboardParameterInDto inDto)
        {
            var value = Parse(inDto);

            if (IsValid(value))
            {
                Apply(value);
            }
            else
            {
                throw new ArgumentException($"Parameter <{GetType()} with value <{value}> is not valid");
            }
        }

        public abstract void Apply(T value);

        public abstract bool IsValid(T value);

        private T Parse(KeyboardParameterInDto value)
        {
            var inDto = JsonConvert.DeserializeObject<EventInDto<T>>(value.Data);

            return inDto.Value;
        }
    }
}
