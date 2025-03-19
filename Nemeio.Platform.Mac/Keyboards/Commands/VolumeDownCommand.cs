using System.Collections.Generic;
using Nemeio.Core;
using Nemeio.Mac.Native;
using KeyboardCommand = Nemeio.Core.Models.SystemKeyboardCommand.SystemKeyboardCommand;

namespace Nemeio.Platform.Mac.Keyboards.Commands
{
    public class VolumeDownCommand : KeyboardCommand
    {
        private const float DecreaseStep = 0.1f;

        public override bool IsTriggered(IList<string> keys)
        {
            return keys.Contains(KeyboardLiterals.VolumeDown);
        }

        public override void Execute() => ExtendedTools.DecreaseVolume(DecreaseStep);
    }
}
