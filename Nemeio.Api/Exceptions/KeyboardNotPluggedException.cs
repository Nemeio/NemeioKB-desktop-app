using System;
using System.Runtime.Serialization;

namespace Nemeio.Api.Exceptions
{
    public class KeyboardNotPluggedException : Exception
    {
        public KeyboardNotPluggedException() { }

        public KeyboardNotPluggedException(string message) : base(message) { }

        public KeyboardNotPluggedException(string message, Exception innerException) : base(message, innerException) { }

        protected KeyboardNotPluggedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
