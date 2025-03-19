using Nemeio.Core.PackageUpdater;

namespace Nemeio.Presentation.Menu.Version
{
    public class VersionSection
    {
        public string Title { get; private set; }
        public string Stm32VersionTitle { get; private set; }
        public string BluetoothLEVersionTitle { get; private set; }
        public string IteVersionTitle { get; private set; }
        public string ApplicationVersionTitle { get; private set; }
        public string UpdateStatus { get; private set; }
        public PackageUpdateState UpdateKind { get; private set; }
        public bool KeyboardIsPlugged { get; private set; }

        public VersionSection(string title, string stm32VersionTitle, string bluetoothLEVersionTitle, string iteVersionTitle, string applicationVersionTitle, string updateStatus, PackageUpdateState kind, bool keyboardIsPlugged)
        {
            Title = title;
            Stm32VersionTitle = stm32VersionTitle;
            BluetoothLEVersionTitle = bluetoothLEVersionTitle;
            IteVersionTitle = iteVersionTitle;
            ApplicationVersionTitle = applicationVersionTitle;
            UpdateStatus = updateStatus;
            UpdateKind = kind;
            KeyboardIsPlugged = keyboardIsPlugged;
        }
    }
}
