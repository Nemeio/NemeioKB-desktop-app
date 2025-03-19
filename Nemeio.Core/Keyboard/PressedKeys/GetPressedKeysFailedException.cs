using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.PressedKeys
{
    [Serializable]
    public class GetPressedKeysFailedException : Exception
    {
        public GetPressedKeysFailedException() { }

        public GetPressedKeysFailedException(string message)
            : base(message) { }

        public GetPressedKeysFailedException(string message, Exception innerException)
            : base(message, innerException) { }

        protected GetPressedKeysFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
