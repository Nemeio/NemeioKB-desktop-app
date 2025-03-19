using System.Collections.Generic;
using Nemeio.Core;
using Nemeio.Windows.Win32;
using KeyboardCommand = Nemeio.Core.Models.SystemKeyboardCommand.SystemKeyboardCommand;

namespace Nemeio.Platform.Windows.Keyboards.Commands
{
    public class StandByCommand : KeyboardCommand
    {
        public override bool IsTriggered(IList<string> keys)
        {
            return keys.Contains(KeyboardLiterals.StandBy);
        }

        public override void Execute()
        {
            WinUser32.SetSuspendState(false, true, true);
        }
    }
}
