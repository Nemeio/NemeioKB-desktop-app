using Nemeio.Wpf.Windows;

namespace Nemeio.Wpf.Modals
{
    public sealed class WinKeyboardInitErrorModal : WinModalWindow<KeyboardInitErrorModal>
    {
        public override KeyboardInitErrorModal CreateNativeWindow() => new KeyboardInitErrorModal();
    }
}
