using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal sealed class PowerOffTimeUsbDisconnectedParameter : UintParameter
    {
        public const int PowerManagementTimingMaxValue = int.MaxValue / 1000000;

        public PowerOffTimeUsbDisconnectedParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(uint value)
        {
            _parameters.PowerOffTimeUSBDisconnected = value;
        }

        public override bool IsValid(uint value)
        {
            return (value > _parameters.SleepTimeUSBDisconnected) && (value <= PowerManagementTimingMaxValue);
        }
    }
}
