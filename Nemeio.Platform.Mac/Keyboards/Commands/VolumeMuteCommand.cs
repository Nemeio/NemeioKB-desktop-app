using System.Collections.Generic;
using Nemeio.Core;
using Nemeio.Mac.Native;
using KeyboardCommand = Nemeio.Core.Models.SystemKeyboardCommand.SystemKeyboardCommand;

namespace Nemeio.Platform.Mac.Keyboards.Commands
{
    public class VolumeMuteCommand : KeyboardCommand
    {
        public override bool IsTriggered(IList<string> keys)
        {
            return keys.Contains(KeyboardLiterals.VolumeMute);
        }

        public override void Execute() => ExtendedTools.ReverseMute();
    }
}
