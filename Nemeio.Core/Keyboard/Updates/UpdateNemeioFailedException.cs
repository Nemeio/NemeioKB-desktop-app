using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.Updates
{
    [Serializable]
    public class UpdateNemeioFailedException : Exception
    {
        public UpdateNemeioFailedException() { }

        public UpdateNemeioFailedException(string message) 
            : base(message) { }

        public UpdateNemeioFailedException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected UpdateNemeioFailedException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
