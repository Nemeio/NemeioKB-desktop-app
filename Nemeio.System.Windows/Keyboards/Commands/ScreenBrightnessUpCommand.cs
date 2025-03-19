using System.Collections.Generic;
using Nemeio.Core;

namespace Nemeio.Platform.Windows.Keyboards.Commands
{
    public class ScreenBrightnessUpCommand : ScreenBrightnessCommand
    {
        private const int IncreaseStep = 2;

        public override bool IsTriggered(IList<string> keys)
        {
            return keys.Contains(KeyboardLiterals.LightUp);
        }

        public override void Execute()
        {
            var currentValue = GetCurrentBrightness();

            SetCurrentBrightness(currentValue + IncreaseStep);
        }
    }
}
