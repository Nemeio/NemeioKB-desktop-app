using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal sealed class SleepTimeUsbDisconnectedParameter : UintParameter
    {
        public SleepTimeUsbDisconnectedParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(uint value)
        {
            _parameters.SleepTimeUSBDisconnected = value;
        }

        public override bool IsValid(uint value)
        {
            return (value > _parameters.InactiveTimeUSBDisconnected) && (value < _parameters.PowerOffTimeUSBDisconnected);
        }
    }
}
