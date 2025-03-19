using Nemeio.Core.Systems.Hid;
using Nemeio.Platform.Windows;

namespace Nemeio.Platform.Hid.Windows.Keyboards
{
    public sealed class WinSystemModifierDelegate : ISystemModifierDelegate
    {
        public bool IsModifierKey(string key) => WinKeyboardConstants.IsModifierKey(key);
    }
}
