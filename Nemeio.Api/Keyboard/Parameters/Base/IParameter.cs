using Nemeio.Api.Dto.In.Events;

namespace Nemeio.Api.Keyboard.Parameters.Base
{
    internal interface IParameter
    {
        void Update(KeyboardParameterInDto inDto);
    }
}
