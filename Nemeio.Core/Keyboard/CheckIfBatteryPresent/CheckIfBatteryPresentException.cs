using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.CheckIfBatteryPresent
{
    [Serializable]
    public class CheckIfBatteryPresentException : Exception
    {
        public CheckIfBatteryPresentException() { }

        public CheckIfBatteryPresentException(string message)
            : base(message) { }

        public CheckIfBatteryPresentException(string message, Exception innerException)
            : base(message, innerException) { }

        protected CheckIfBatteryPresentException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
