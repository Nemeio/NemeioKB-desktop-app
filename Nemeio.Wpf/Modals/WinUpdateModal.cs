using Nemeio.Wpf.Windows;

namespace Nemeio.Wpf.Modals
{
    public class WinUpdateModal : WinModalWindow<UpdateWindow>
    {
        public override UpdateWindow CreateNativeWindow() => new UpdateWindow();
    }
}
