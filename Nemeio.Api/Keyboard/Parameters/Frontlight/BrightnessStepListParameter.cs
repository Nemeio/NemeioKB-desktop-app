using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal sealed class BrightnessStepListParameter : ByteListParameter
    {
        private const int MinimumStepValue = 0;
        private const int MaximumStepValue = 100;

        public BrightnessStepListParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(List<byte> values)
        {
            var ledMaxPowerLever = _parameters.LedPowerMaxLevel;

            if (!values.Contains(ledMaxPowerLever))
            {
                values.Add(ledMaxPowerLever);
            }

            foreach (var value in values)
            {
                if (value < MinimumStepValue || value > MaximumStepValue)
                {
                    throw new ArgumentException($"Value must be greater than <{MinimumStepValue}> and lower than <{MaximumStepValue}>");
                }
            }

            var list = values
                .Distinct()
                .ToList();

            list.Sort();

            _parameters.BrightnessStepList = list;
        }

        public override bool IsValid(List<byte> value) => true;
    }
}
