using FluentAssertions;
using Nemeio.Api.Keyboard.Parameters;
using Nemeio.Core.Keyboard.Parameters;
using NUnit.Framework;

namespace Nemeio.Api.Test.Keyboards.Parameters
{
    [TestFixture]
    internal class LedPowerMaxLevelParameterShould
    {
        [Test]
        public void LedPowerMaxLevelParameter_IsValid_WhenValueIsHigherThanOneHundred_ReturnFalse()
        {
            var keyboardParameter = new KeyboardParameters();
            var parameter = new LedPowerMaxLevelParameter(keyboardParameter);

            var isValid = parameter.IsValid(101);

            isValid.Should().BeFalse();
        }
    }
}
