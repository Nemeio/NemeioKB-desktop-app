using System.Collections.Generic;
using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal abstract class ByteListParameter : Parameter<List<byte>>
    {
        protected ByteListParameter(KeyboardParameters parameters) 
            : base(parameters) { }
    }
}
