using System;
using System.Runtime.Serialization;
using Nemeio.Core.Errors;

namespace Nemeio.Core.Exceptions
{
    [Serializable]
    public class CoreException : Exception
    {
        public ErrorCode ErrorCode { get; }

        public CoreException(ErrorCode errorCode) 
        {
            ErrorCode = errorCode;
        }

        public CoreException(ErrorCode errorCode, string message) 
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public CoreException(ErrorCode errorCode, string message, Exception innerException) 
            : base(message, innerException) 
        {
            ErrorCode = errorCode;
        }

        protected CoreException(ErrorCode errorCode, SerializationInfo info, StreamingContext context) 
            : base(info, context) 
        {
            ErrorCode = errorCode;
        }
    }
}
