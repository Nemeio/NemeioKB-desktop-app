using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal sealed class BrightnessStepParameter : ByteParameter
    {
        public BrightnessStepParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(byte value)
        {
            _parameters.BrigthnessStep = value;
        }

        public override bool IsValid(byte value) => true;
    }
}
