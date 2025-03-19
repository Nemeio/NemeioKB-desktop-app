using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal sealed class LowBatteryBlinkOffDelayParameter : UshortParameter
    {
        public LowBatteryBlinkOffDelayParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(ushort value)
        {
            _parameters.LowBatteryBlinkOffDelayMs = value;
        }

        public override bool IsValid(ushort value) => true;
    }
}
