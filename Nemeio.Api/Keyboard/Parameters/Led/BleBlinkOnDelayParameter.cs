using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal sealed class BleBlinkOnDelayParameter : UshortParameter
    {
        public BleBlinkOnDelayParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(ushort value)
        {
            _parameters.BleBlinkOnDelayMs = value;
        }

        public override bool IsValid(ushort value) => true;
    }
}
