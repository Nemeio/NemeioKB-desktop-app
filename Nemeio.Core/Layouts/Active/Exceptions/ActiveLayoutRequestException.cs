using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Layouts.Active.Exceptions
{
    public sealed class ActiveLayoutRequestException : Exception
    {
        public ActiveLayoutRequestException() { }

        public ActiveLayoutRequestException(string message) 
            : base(message) { }

        public ActiveLayoutRequestException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }

        public ActiveLayoutRequestException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
