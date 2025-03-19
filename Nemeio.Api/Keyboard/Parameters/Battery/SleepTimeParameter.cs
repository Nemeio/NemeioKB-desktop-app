using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal sealed class SleepTimeParameter : UintParameter
    {
        public const int PowerManagementTimingMaxValue = int.MaxValue / 1000000;

        public SleepTimeParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(uint value)
        {
            _parameters.SleepTime = value;
        }

        public override bool IsValid(uint value)
        {
            return value > _parameters.InactiveTime && value <= PowerManagementTimingMaxValue;
        }
    }
}
