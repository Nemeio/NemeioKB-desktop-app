using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal sealed class InactiveTimeParameter : UintParameter
    {
        public InactiveTimeParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(uint value)
        {
            _parameters.InactiveTime = value;
        }

        public override bool IsValid(uint value)
        {
            return value < _parameters.SleepTime;
        }
    }
}
