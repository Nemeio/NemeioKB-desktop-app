using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.SetProvisionning
{
    [Serializable]
    public class SetProvisionningException : Exception
    {
        public SetProvisionningException() { }

        public SetProvisionningException(string message)
            : base(message) { }

        public SetProvisionningException(string message, Exception innerException)
            : base(message, innerException) { }

        protected SetProvisionningException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
