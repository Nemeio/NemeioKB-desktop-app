using System;
using System.Globalization;
using Nemeio.Core.Applications.Manifest;
using Nemeio.Core.Services.AppSettings;

namespace Nemeio.Core.Applications
{
    public class ApplicationSettingsProvider : IApplicationSettingsProvider
    {
        private IApplicationSettingsDbRepository _settingsDbRepository;

        public event EventHandler<EventArgs> LanguageChanged;
        public event EventHandler<EventArgs> AugmentedImageEnableChanged;
        public event EventHandler<EventArgs> ShowGrantPrivilegeWindowChanged;
        public event EventHandler<EventArgs> UpdateToChanged;
        public event EventHandler<EventArgs> LastRollbackManifestStringChanged;

        public ApplicationSettingsProvider(IApplicationSettingsDbRepository appSettings)
        {
            _settingsDbRepository = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public ApplicationSettings ApplicationSettings
        {
            get => _settingsDbRepository.ReadApplicationSettings();
        }

        public CultureInfo Language
        {
            get => ApplicationSettings.Language;
            set
            {
                var settings = ApplicationSettings;
                settings.Language = value;

                _settingsDbRepository.SaveApplicationSettings(settings);

                LanguageChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool AugmentedImageEnable
        {
            get => ApplicationSettings.AugmentedImageEnable;
            set
            {
                var settings = ApplicationSettings;
                settings.AugmentedImageEnable = value;

                _settingsDbRepository.SaveApplicationSettings(settings);

                AugmentedImageEnableChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool ShowGrantPrivilegeWindow
        {
            get => ApplicationSettings.ShowGrantPrivilegeWindow;
            set
            {
                var settings = ApplicationSettings;
                settings.ShowGrantPrivilegeWindow = value;

                _settingsDbRepository.SaveApplicationSettings(settings);

                ShowGrantPrivilegeWindowChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public Version UpdateTo
        {
            get => ApplicationSettings.UpdateTo;
            set
            {
                var settings = ApplicationSettings;
                settings.UpdateTo = value;

                _settingsDbRepository.SaveApplicationSettings(settings);

                UpdateToChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public string LastRollbackManifestString
        {
            get => ApplicationSettings.LastRollbackManifestString;
            set
            {
                var settings = ApplicationSettings;
                settings.LastRollbackManifestString = value;

                _settingsDbRepository.SaveApplicationSettings(settings);

                LastRollbackManifestStringChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
