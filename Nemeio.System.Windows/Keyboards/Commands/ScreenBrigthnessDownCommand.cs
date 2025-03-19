using System.Collections.Generic;
using Nemeio.Core;

namespace Nemeio.Platform.Windows.Keyboards.Commands
{
    public class ScreenBrigthnessDownCommand : ScreenBrightnessCommand
    {
        private const int DecreaseStep = 2;

        public override bool IsTriggered(IList<string> keys)
        {
            return keys.Contains(KeyboardLiterals.LightLow);
        }

        public override void Execute()
        {
            var currentValue = GetCurrentBrightness();

            SetCurrentBrightness(currentValue - DecreaseStep);
        }
    }
}
