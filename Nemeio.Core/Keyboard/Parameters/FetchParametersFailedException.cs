using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.Parameters
{
    [Serializable]
    public class FetchParametersFailedException : Exception
    {
        public FetchParametersFailedException() { }

        public FetchParametersFailedException(string message) 
            : base(message) { }

        public FetchParametersFailedException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected FetchParametersFailedException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
