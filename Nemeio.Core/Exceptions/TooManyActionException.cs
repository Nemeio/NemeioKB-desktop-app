using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Exceptions
{
    public class TooManyActionException : Exception
    {
        public TooManyActionException() { }

        public TooManyActionException(string message) : base(message) { }

        public TooManyActionException(string message, Exception innerException) : base(message, innerException) { }

        protected TooManyActionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
