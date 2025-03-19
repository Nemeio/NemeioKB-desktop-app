using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters.Screen
{
    internal sealed class BlackBackgroundCleanRefreshPeriodParameter : ByteParameter
    {
        public BlackBackgroundCleanRefreshPeriodParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(byte value)
        {
            _parameters.BlackBackgroundCleanRefreshPeriod = value;
        }

        public override bool IsValid(byte value) => true;
    }
}
