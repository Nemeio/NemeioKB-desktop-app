using Nemeio.Wpf.Windows;

namespace Nemeio.Wpf.Modals
{
    public class WinAdministratorModal : WinModalWindow<AskAdminRightWindow>
    {
        private readonly string _processName;

        public WinAdministratorModal(string processName)
        {
            _processName = processName ?? string.Empty;
        }

        public override AskAdminRightWindow CreateNativeWindow() => new AskAdminRightWindow(_processName);
    }
}
