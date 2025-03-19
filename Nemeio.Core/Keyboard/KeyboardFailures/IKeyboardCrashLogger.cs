using System.Collections.Generic;
using Nemeio.Core.DataModels;

namespace Nemeio.Core.Keyboard.KeyboardFailures
{
    public interface IKeyboardCrashLogger
    {
        void WriteKeyboardCrashLog(FirmwareVersions keyboardVersion, IList<KeyboardFailure> keyboardFailures);
    }
}
