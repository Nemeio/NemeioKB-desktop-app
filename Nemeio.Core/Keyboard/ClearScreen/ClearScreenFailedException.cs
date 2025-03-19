using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.ClearScreen
{
    [Serializable]
    public class ClearScreenFailedException : Exception
    {
        public ClearScreenFailedException() { }

        public ClearScreenFailedException(string message)
            : base(message) { }

        public ClearScreenFailedException(string message, Exception innerException)
            : base(message, innerException) { }

        protected ClearScreenFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
