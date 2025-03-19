using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.SetFrontLightPower
{
    [Serializable]
    public class SetFrontLightPowerException : Exception
    {
        public SetFrontLightPowerException() { }

        public SetFrontLightPowerException(string message)
            : base(message) { }

        public SetFrontLightPowerException(string message, Exception innerException)
            : base(message, innerException) { }

        protected SetFrontLightPowerException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
