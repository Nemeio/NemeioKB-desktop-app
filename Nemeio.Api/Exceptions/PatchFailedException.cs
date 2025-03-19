using System;
using System.Runtime.Serialization;

namespace Nemeio.Api.Exceptions
{
    public class PatchFailedException : Exception
    {
        public int ErrorCode { get; private set; } = 99;

        public PatchFailedException() { }

        public PatchFailedException(string message) 
            : base(message) { }

        public PatchFailedException(int errorCode, string message)
            : base(message) => ErrorCode = errorCode;

        public PatchFailedException(string message, Exception innerException) 
            : base(message, innerException) { }

        public PatchFailedException(int errorCode, string message, Exception innerException)
            : base(message, innerException) => ErrorCode = errorCode;

        protected PatchFailedException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
