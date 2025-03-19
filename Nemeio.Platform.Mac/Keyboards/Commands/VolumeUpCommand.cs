using System.Collections.Generic;
using Nemeio.Core;
using Nemeio.Mac.Native;
using KeyboardCommand = Nemeio.Core.Models.SystemKeyboardCommand.SystemKeyboardCommand;

namespace Nemeio.Platform.Mac.Keyboards.Commands
{
    public class VolumeUpCommand : KeyboardCommand
    {
        private const float IncreaseStep = 0.1f;

        public override bool IsTriggered(IList<string> keys)
        {
            return keys.Contains(KeyboardLiterals.VolumeUp);
        }

        public override void Execute() => ExtendedTools.IncreaseVolume(IncreaseStep);
    }
}
