using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.LayoutsIds
{
    [Serializable]
    public class FetchLayoutHashFailedException : Exception
    {
        public FetchLayoutHashFailedException() { }

        public FetchLayoutHashFailedException(string message) 
            : base(message) { }

        public FetchLayoutHashFailedException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected FetchLayoutHashFailedException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
