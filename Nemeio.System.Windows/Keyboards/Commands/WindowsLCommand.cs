using System.Collections.Generic;
using System.Linq;
using Nemeio.Core;
using Nemeio.Windows.Win32;
using KeyboardCommand = Nemeio.Core.Models.SystemKeyboardCommand.SystemKeyboardCommand;

namespace Nemeio.Platform.Windows.Keyboards.Commands
{
    public class WindowsLCommand : KeyboardCommand
    {
        public override bool IsTriggered(IList<string> keys)
        {
            if (keys.Count < 2)
            {
                return false;
            }

            var isWindowsKey = keys.First().Equals(KeyboardLiterals.WindowsKey) || keys.First().Equals(KeyboardLiterals.CMD);
            var isLowerLLetter = keys.ElementAt(1).Equals("l");

            return isWindowsKey && isLowerLLetter;
        }

        public override void Execute()
        {
            WinUser32.LockWorkStation();
        }
    }
}
