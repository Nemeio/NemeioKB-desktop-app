using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal abstract class UshortParameter : Parameter<ushort>
    {
        protected UshortParameter(KeyboardParameters parameters) 
            : base(parameters) { }
    }
}
