using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.SerialNumber
{
    [Serializable]
    public class FetchSerialNumberFailedException : Exception
    {
        public FetchSerialNumberFailedException() { }

        public FetchSerialNumberFailedException(string message) 
            : base(message) { }

        public FetchSerialNumberFailedException(string message, Exception innerException)
            : base(message, innerException) { }

        protected FetchSerialNumberFailedException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
