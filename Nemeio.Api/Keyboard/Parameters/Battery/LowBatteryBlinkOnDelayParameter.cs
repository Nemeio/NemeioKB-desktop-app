using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal sealed class LowBatteryBlinkOnDelayParameter : UshortParameter
    {
        public LowBatteryBlinkOnDelayParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(ushort value)
        {
            _parameters.LowBatteryBlinkOnDelayMs = value;
        }

        public override bool IsValid(ushort value) => true;
    }
}
