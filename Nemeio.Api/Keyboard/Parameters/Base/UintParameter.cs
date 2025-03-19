using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal abstract class UintParameter : Parameter<uint>
    {
        protected UintParameter(KeyboardParameters parameters) 
            : base(parameters) { }
    }
}
