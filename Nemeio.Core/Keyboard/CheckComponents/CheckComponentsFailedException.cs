using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.SetLed
{
    [Serializable]
    public class CheckComponentsFailedException : Exception
    {
        public CheckComponentsFailedException() { }

        public CheckComponentsFailedException(string message)
            : base(message) { }

        public CheckComponentsFailedException(string message, Exception innerException)
            : base(message, innerException) { }

        protected CheckComponentsFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
