using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.Communication.Exceptions
{
    [Serializable]
    public abstract class KeyboardCommunicationException : Exception
    {
        public KeyboardCommunicationException() { }

        public KeyboardCommunicationException(string message) 
            : base(message) { }

        public KeyboardCommunicationException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected KeyboardCommunicationException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
