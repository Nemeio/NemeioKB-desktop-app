using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal sealed class CleanRefreshPeriodParameter : ByteParameter
    {
        public CleanRefreshPeriodParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(byte value)
        {
            _parameters.CleanRefreshPeriod = value;
        }

        public override bool IsValid(byte value) => true;
    }
}
