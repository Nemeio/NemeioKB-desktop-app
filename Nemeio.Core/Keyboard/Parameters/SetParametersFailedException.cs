using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.Parameters
{
    [Serializable]
    public class SetParametersFailedException : Exception
    {
        public SetParametersFailedException() { }

        public SetParametersFailedException(string message) 
            : base(message) { }

        public SetParametersFailedException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected SetParametersFailedException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
