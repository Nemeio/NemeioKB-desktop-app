using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters.Base
{
    internal abstract class BooleanParameter : Parameter<bool>
    {
        protected BooleanParameter(KeyboardParameters parameters) 
            : base(parameters) { }
    }
}
