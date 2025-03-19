using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.SetLed
{
    [Serializable]
    public class SetLedFailedException : Exception
    {
        public SetLedFailedException() { }

        public SetLedFailedException(string message)
            : base(message) { }

        public SetLedFailedException(string message, Exception innerException)
            : base(message, innerException) { }

        protected SetLedFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
