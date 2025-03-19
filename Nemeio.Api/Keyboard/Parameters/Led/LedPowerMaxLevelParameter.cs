using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal class LedPowerMaxLevelParameter : ByteParameter
    {
        private const int MaxPower = 100;

        public LedPowerMaxLevelParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(byte value)
        {
            var previousValue = _parameters.LedPowerMaxLevel;

            _parameters.LedPowerMaxLevel = value;

            if (_parameters.BrightnessStepList.Contains(previousValue))
            {
                _parameters.BrightnessStepList.Remove(previousValue);
            } 

            if (!_parameters.BrightnessStepList.Contains(value))
            {
                _parameters.BrightnessStepList.Add(value);
                _parameters.BrightnessStepList.Sort();
            }
        }

        public override bool IsValid(byte value)
        {
            return value <= MaxPower;
        }
    }
}
