using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Errors;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services;
using Nemeio.Core.Settings.Providers;
using Nemeio.Core.Tools.Watchers;

namespace Nemeio.Core.Settings
{
    public sealed class SettingsHolder : ISettingsHolder
    {
        public const string SettingsFilename = "nemeio.settings";

        private readonly ILogger _logger;
        private readonly IDocument _documentService;
        private readonly IErrorManager _errorManager;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IWatcherFactory _watcherFactory;
        private readonly IFontProvider _fontProvider;
        private IWatcher _settingsFileWatcher;
        private ISettings _settings;
        private string SettingsFilePath => Path.Combine(_documentService.UserNemeioFolder, SettingsFilename);

        public event EventHandler OnSettingsUpdated;

        public ISettings Settings
        {
            get => _settings;
            private set
            {
                _settings = value;
                OnSettingsUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public SettingsHolder(ILoggerFactory loggerFactory, IDocument documentService, IErrorManager errorManager, ISettingsProvider settingsProvider, IWatcherFactory watcherFactory, IFontProvider fontProvider) 
        {
            _logger = loggerFactory.CreateLogger<SettingsHolder>();
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
            _errorManager = errorManager ?? throw new ArgumentNullException(nameof(errorManager));
            _settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            _watcherFactory = watcherFactory ?? throw new ArgumentNullException(nameof(watcherFactory));
            //Can be Null
            _fontProvider = fontProvider; 

            TryLoadSettingsFile();
        }

        private void TryLoadSettingsFile()
        {
            _logger.LogInformation($"Try load settings file");

            try
            {
                Settings = _settingsProvider.LoadSettings(SettingsFilePath);

                if (_settingsFileWatcher == null)
                {
                    _logger.LogInformation($"Register file watcher");

                    //  We register watcher
                    _settingsFileWatcher = _watcherFactory.CreateFileWatcher(SettingsFilePath);
                    _settingsFileWatcher.OnChanged += SettingsFileWatcher_OnChanged;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    _errorManager.GetFullErrorMessage(ErrorCode.CoreLoadDevelopmentSettingsFailed)
                );
            }
        }

        private void ReloadSettingsFile()
        {
            _logger.LogInformation("Reload settings");

            try
            {
                var settings = _settingsProvider.LoadSettings(SettingsFilePath);
                
                Settings.Update(settings);
                _fontProvider.RefreshFonts();

                OnSettingsUpdated?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    _errorManager.GetFullErrorMessage(ErrorCode.CoreLoadDevelopmentSettingsFailed)
                );
            }
        }

        private void SettingsFileWatcher_OnChanged(object sender, EventArgs e) => ReloadSettingsFile();
    }
}
