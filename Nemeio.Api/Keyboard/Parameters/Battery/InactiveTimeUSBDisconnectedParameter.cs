using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal sealed class InactiveTimeUsbDisconnectedParameter : UintParameter
    {
        public InactiveTimeUsbDisconnectedParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(uint value)
        {
            _parameters.InactiveTimeUSBDisconnected = value;
        }

        public override bool IsValid(uint value)
        {
            return value < _parameters.SleepTimeUSBDisconnected;
        }
    }
}
