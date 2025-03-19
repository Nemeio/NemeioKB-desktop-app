using Nemeio.Api.Keyboard.Parameters.Base;
using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal sealed class HighQualityModifierParameter : BooleanParameter
    {
        public HighQualityModifierParameter(KeyboardParameters parameters) 
            : base(parameters) { }

        public override void Apply(bool value)
        {
            _parameters.HighQualityPercent = value;
        }

        public override bool IsValid(bool value) => true;
    }
}
