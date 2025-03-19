using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal sealed class ButtonRepeatLongPressDelayParameter : UshortParameter
    {
        public ButtonRepeatLongPressDelayParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(ushort value)
        {
            _parameters.ButtonRepeatLongPressDelay = value;
        }

        public override bool IsValid(ushort value) => true;
    }
}
