using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.Configurations.Add
{
    [Serializable]
    public class AddConfigurationFailedException : Exception
    {
        public AddConfigurationFailedException() { }

        public AddConfigurationFailedException(string message) 
            : base(message) { }

        public AddConfigurationFailedException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected AddConfigurationFailedException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
