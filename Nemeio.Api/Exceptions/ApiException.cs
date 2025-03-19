using System;
using System.Runtime.Serialization;
using Nemeio.Core.Errors;

namespace Nemeio.Api.Exceptions
{
    [Serializable]
    public class ApiException : Exception
    {
        public ErrorCode ErrorCode { get; }

        public ApiException(ErrorCode errorCode) 
        {
            ErrorCode = errorCode;
        }

        public ApiException(ErrorCode errorCode, string message) 
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public ApiException(ErrorCode errorCode, string message, Exception innerException) 
            : base(message, innerException) 
        {
            ErrorCode = errorCode;
        }

        protected ApiException(ErrorCode errorCode, SerializationInfo info, StreamingContext context) 
            : base(info, context) 
        {
            ErrorCode = errorCode;
        }
    }
}
