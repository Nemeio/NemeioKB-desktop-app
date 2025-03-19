using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.Configurations.Delete
{
    [Serializable]
    public class DeleteConfigurationFailedException : Exception
    {
        public DeleteConfigurationFailedException() { }

        public DeleteConfigurationFailedException(string message)
            : base(message) { }

        public DeleteConfigurationFailedException(string message, Exception innerException)
            : base(message, innerException) { }

        protected DeleteConfigurationFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
