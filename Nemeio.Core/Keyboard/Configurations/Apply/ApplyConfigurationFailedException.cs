using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.Configurations.Apply
{
    [Serializable]
    public class ApplyConfigurationFailedException : Exception
    {
        public ApplyConfigurationFailedException() { }

        public ApplyConfigurationFailedException(string message)
            : base(message) { }

        public ApplyConfigurationFailedException(string message, Exception innerException)
            : base(message, innerException) { }

        protected ApplyConfigurationFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
