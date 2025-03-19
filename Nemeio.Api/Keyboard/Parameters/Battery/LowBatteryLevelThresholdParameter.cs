using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal sealed class LowBatteryLevelThresholdParameter : ByteParameter
    {
        public LowBatteryLevelThresholdParameter(KeyboardParameters parameters)
            : base(parameters) { }

        public override void Apply(byte value)
        {
            _parameters.LowBatteryLevelThresholdPercent = value;
        }

        public override bool IsValid(byte value) => value >= 0 && value <= 100;
    }
}
