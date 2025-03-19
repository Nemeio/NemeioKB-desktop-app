using Nemeio.Wpf.Windows;

namespace Nemeio.Wpf.Modals
{
    public sealed class WinFactoryResetModal : WinModalWindow<FactoryResetModal>
    {
        public override FactoryResetModal CreateNativeWindow() => new FactoryResetModal();
    }
}
