using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal sealed class BleBlinkOffDelayParameter : UshortParameter
    {
        public BleBlinkOffDelayParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(ushort value)
        {
            _parameters.BleBlinkOffDelayMs = value;
        }

        public override bool IsValid(ushort value) => true;
    }
}
