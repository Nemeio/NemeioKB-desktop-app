using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Exceptions
{
    [Serializable]
    public class ForbiddenActionException : Exception
    {
        public ForbiddenActionException() { }

        public ForbiddenActionException(string message)
            : base(message) { }

        public ForbiddenActionException(string message, Exception innerException)
            : base(message, innerException) { }

        protected ForbiddenActionException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
