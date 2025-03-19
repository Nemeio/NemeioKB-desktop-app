using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.Version
{
    [Serializable]
    public class FetchVersionsFailedException : Exception
    {
        public FetchVersionsFailedException() { }

        public FetchVersionsFailedException(string message) 
            : base(message) { }

        public FetchVersionsFailedException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected FetchVersionsFailedException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
