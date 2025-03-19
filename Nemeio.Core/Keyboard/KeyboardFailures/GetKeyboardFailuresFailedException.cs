using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.KeyboardFailures
{
    [Serializable]
    public class GetKeyboardFailuresFailedException : Exception
    {
        public GetKeyboardFailuresFailedException() { }

        public GetKeyboardFailuresFailedException(string message) 
            : base(message) { }

        public GetKeyboardFailuresFailedException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected GetKeyboardFailuresFailedException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
