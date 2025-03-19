using System;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Managers;
using Nemeio.Presentation.Menu.Tools;
using Nemeio.Presentation.PackageUpdater.UI;

namespace Nemeio.Presentation.Menu.Version
{
    public class VersionSectionBuilder : SectionBuilder<VersionSection, VersionInformation>
    {
        private const string StmFirmwareHeader = "Keyboard Stm firmware : ";
        private const string NrfFirmwareHeader = "Keyboard Nrf firmware : ";
        private const string IteFirmwareHeader = "Keyboard Ite firmware : ";
        private const string ApplicationHeader = "Application : ";

        private readonly IPackageUpdaterMessageFactory _messageFactory;

        public VersionSectionBuilder(ILanguageManager languageManager, IPackageUpdaterMessageFactory messageFactory) 
            : base(languageManager) 
        {
            _messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
        }

        public override VersionSection Build(VersionInformation obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var section = new VersionSection(
                title: BuildTitle(obj),
                stm32VersionTitle: $"{StmFirmwareHeader} {obj.Stm32Version}",
                bluetoothLEVersionTitle: $"{NrfFirmwareHeader} {obj.BluetoothLEVersion}",
                iteVersionTitle: $"{IteFirmwareHeader} {obj.IteVersion}",
                applicationVersionTitle: $"{ApplicationHeader} {obj.ApplicationVersion}",
                updateStatus: LanguageManager.GetLocalizedValue(_messageFactory.GetStatusMessageForCurrentState()),
                kind: obj.UpdateStatus,
                keyboardIsPlugged: obj.KeyboardIsPlugged
            );

            return section;
        }

        private string BuildTitle(VersionInformation versions)
        {
            var commonVersion = LanguageManager.GetLocalizedValue(StringId.CommonVersion);
            var text = string.Format(commonVersion, versions.ApplicationVersion);

            return text;
        }
    }
}
