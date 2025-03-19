using Nemeio.Core.DataModels;
using Nemeio.Core.PackageUpdater;

namespace Nemeio.Presentation.Menu.Version
{
    public sealed class VersionInformation
    {
        public VersionProxy ApplicationVersion { get; set; }
        public VersionProxy Stm32Version { get; set; }
        public string IteVersion { get; set; }
        public VersionProxy BluetoothLEVersion { get; set; }
        public PackageUpdateState UpdateStatus { get; set; }
        public bool KeyboardIsPlugged { get; set; }
    }
}
