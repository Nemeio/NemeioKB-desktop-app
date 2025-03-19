using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.CommunicationMode
{
    [Serializable]
    public class SetCommunicationModeFailedException : Exception
    {
        public SetCommunicationModeFailedException() { }

        public SetCommunicationModeFailedException(string message) 
            : base(message) { }

        public SetCommunicationModeFailedException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected SetCommunicationModeFailedException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
