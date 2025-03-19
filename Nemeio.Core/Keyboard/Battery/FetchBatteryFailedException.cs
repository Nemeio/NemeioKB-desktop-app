using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.Battery
{
    [Serializable]
    public class FetchBatteryFailedException : Exception
    {
        public FetchBatteryFailedException() { }

        public FetchBatteryFailedException(string message) 
            : base(message) { }

        public FetchBatteryFailedException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected FetchBatteryFailedException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
