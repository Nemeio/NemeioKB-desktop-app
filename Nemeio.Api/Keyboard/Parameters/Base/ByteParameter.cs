using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal abstract class ByteParameter : Parameter<byte>
    {
        protected ByteParameter(KeyboardParameters parameters) 
            : base(parameters) { }
    }
}
