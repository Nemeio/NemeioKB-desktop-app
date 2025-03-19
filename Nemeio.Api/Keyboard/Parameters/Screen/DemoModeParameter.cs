using System;
using Nemeio.Api.Keyboard.Parameters.Base;
using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal sealed class DemoModeParameter : BooleanParameter
    {
        public DemoModeParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(bool value)
        {
            _parameters.DemoMode = Convert.ToBoolean(value);
        }

        public override bool IsValid(bool value) => true;
    }
}
