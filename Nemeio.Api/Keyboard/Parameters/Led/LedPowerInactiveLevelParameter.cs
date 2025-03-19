using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal sealed class LedPowerInactiveLevelParameter : ByteParameter
    {
        private const int MaxPower = 100;

        public LedPowerInactiveLevelParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(byte value)
        {
            _parameters.LedPowerInactiveLevel = value;
        }

        public override bool IsValid(byte value)
        {
            return value <= MaxPower;
        }
    }
}
