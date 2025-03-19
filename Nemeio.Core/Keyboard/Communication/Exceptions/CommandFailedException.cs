using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.Communication.Exceptions
{
    [Serializable]
    public class CommandFailedException : KeyboardCommunicationException
    {
        public CommandFailedException() { }

        public CommandFailedException(string message) 
            : base(message) { }

        public CommandFailedException(string message, Exception innerException) 
            : base(message, innerException) { }

        public CommandFailedException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
