using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.Communication.Exceptions
{
    [Serializable]
    public class NoKeyboardResponseException : KeyboardCommunicationException
    {
        public NoKeyboardResponseException() { }

        public NoKeyboardResponseException(string message) 
            : base(message) { }

        public NoKeyboardResponseException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected NoKeyboardResponseException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
