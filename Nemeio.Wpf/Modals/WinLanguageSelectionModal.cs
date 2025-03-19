using Nemeio.Wpf.Windows;

namespace Nemeio.Wpf.Modals
{
    public sealed class WinLanguageSelectionModal : WinModalWindow<LanguageSelectionWindow>
    {
        public override LanguageSelectionWindow CreateNativeWindow() => new LanguageSelectionWindow();
    }
}
