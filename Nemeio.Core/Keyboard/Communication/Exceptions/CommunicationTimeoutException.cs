using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.Communication.Exceptions
{
    [Serializable]
    public class CommunicationTimeoutException : KeyboardCommunicationException
    {
        public CommunicationTimeoutException() { }

        public CommunicationTimeoutException(string message) 
            : base(message) { }

        public CommunicationTimeoutException(string message, Exception innerException) 
            : base(message, innerException) { }

        public CommunicationTimeoutException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
