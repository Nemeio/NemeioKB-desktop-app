using System;
using System.Runtime.Serialization;

namespace Nemeio.Tools.Core
{
    public abstract class ApplicationException<T> : Exception where T : Enum
    {
        public T ExitCode { get; private set; }

        protected ApplicationException(T exitCode)
        {
            ExitCode = exitCode;
        }

        protected ApplicationException(T exitCode, string message) 
            : base(message)
        {
            ExitCode = exitCode;
        }

        protected ApplicationException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }

        protected ApplicationException(T exitCode, string message, Exception innerException) 
            : base(message, innerException)
        {
            ExitCode = exitCode;
        }
    }
}
