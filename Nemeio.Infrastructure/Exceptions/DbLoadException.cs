using System;
using System.Runtime.Serialization;

namespace Nemeio.Infrastructure.Exceptions
{
    [Serializable]
    public class DbLoadException : Exception
    {
        public DbLoadException() { }

        public DbLoadException(string message) : base(message) { }

        public DbLoadException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected DbLoadException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
