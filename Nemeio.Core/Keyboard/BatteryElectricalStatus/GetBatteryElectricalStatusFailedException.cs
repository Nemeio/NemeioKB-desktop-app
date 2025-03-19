using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.BatteryElectricalStatus
{
    [Serializable]
    public class GetBatteryElectricalStatusFailedException : Exception
    {
        public GetBatteryElectricalStatusFailedException() { }

        public GetBatteryElectricalStatusFailedException(string message)
            : base(message) { }

        public GetBatteryElectricalStatusFailedException(string message, Exception innerException)
            : base(message, innerException) { }

        protected GetBatteryElectricalStatusFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
