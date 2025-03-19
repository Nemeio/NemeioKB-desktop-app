using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.SetAdvertising
{
    [Serializable]
    public class SetAdvertisingException : Exception
    {
        public SetAdvertisingException() { }

        public SetAdvertisingException(string message)
            : base(message) { }

        public SetAdvertisingException(string message, Exception innerException)
            : base(message, innerException) { }

        protected SetAdvertisingException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
