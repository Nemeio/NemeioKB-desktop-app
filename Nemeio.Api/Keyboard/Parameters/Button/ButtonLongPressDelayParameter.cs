using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal sealed class ButtonLongPressDelayParameter : UshortParameter
    {
        public ButtonLongPressDelayParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(ushort value)
        {
            _parameters.ButtonLongPressDelay = value;
        }

        public override bool IsValid(ushort value) => true;
    }
}
