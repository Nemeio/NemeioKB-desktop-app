using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Applications;
using Nemeio.Core.DataModels.Locale;

namespace Nemeio.Core.Managers
{
    public class LanguageManager : ILanguageManager
    {
        public const string DefaultLanguageCode = "en-US";

        private readonly ILogger _logger;

        private Language _currentLanguage;
        private CultureInfo _currentCulture;
        private bool _waitForUserSelection;
        private IApplicationSettingsProvider _applicationSettingsManager;

        public event EventHandler<EventArgs> LanguageChanged;

        public event EventHandler<EventArgs> LanguageDueCare;

        public Language CurrentLanguage => _currentLanguage;

        public CultureInfo CurrentCulture => _currentCulture;

        public LanguageManager(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<LanguageManager>();

            //  At startup we load by default english version
            //  And after that, we load language from database

            LoadLocalizableForCulture(new CultureInfo(DefaultLanguageCode));
            RaiseCultureChanged();
        }

        public void Start()
        {
            _logger.LogInformation("LanguageManager.Start()");

            if (_applicationSettingsManager == null)
            {
                _logger.LogError($"ApplicationSettingsManager is not set");

                throw new ArgumentNullException(nameof(_applicationSettingsManager));
            }

            //  Check language in database
            var language = _applicationSettingsManager.Language;
            if (language == null)
            {
                //  No language in database
                //  Until user select a language we let default language
                _waitForUserSelection = true;

                RaiseLanguageDueCare();
            }
            else
            {
                //  User have already choose which language he want to use

                LoadLocalizableForCulture(GetCurrentCultureInfo());
                RaiseCultureChanged();
            }
        }

        public void Stop()
        {
            //  Nothing to do here
        }

        public CultureInfo GetCurrentCultureInfo()
        {
            if (_applicationSettingsManager == null)
            {
                _logger.LogError($"ApplicationSettingsManager is not set");

                throw new ArgumentNullException(nameof(_applicationSettingsManager));
            }

            return _applicationSettingsManager.Language;
        }

        public void SelectMainLanguage(CultureInfo culture)
        {
            if (!_waitForUserSelection)
            {
                throw new InvalidOperationException("User have already selected his language");
            }

            SetCurrentCultureInfo(culture);
        }

        public void SetCurrentCultureInfo(CultureInfo culture)
        {
            if (!_waitForUserSelection && culture.Name == CurrentCulture.Name)
            {
                //  Same language already in use

                return;
            }

            if (_applicationSettingsManager == null)
            {
                _logger.LogError($"ApplicationSettingsManager is not set");

                throw new ArgumentNullException(nameof(_applicationSettingsManager));
            }

            _applicationSettingsManager.Language = culture;

            LoadLocalizableForCulture(culture);
            RaiseCultureChanged();
        }

        public string GetLocalizedValue(StringId key)
        {
            var translatedValue = _currentLanguage.Entries.FirstOrDefault(x => x.Key == key);

            return translatedValue?.TranslateValue ?? String.Empty;
        }

        public IEnumerable<CultureInfo> GetSupportedLanguages()
        {
            var languagesNames = GetCoreAssembly()
                                    .GetManifestResourceNames()
                                    .Where(x => x.StartsWith(GetResourcesAssemblyPath()))
                                    .Select(x => GetResourceFileNameWithoutExtension(x))
                                    .Select(x => new CultureInfo(x));

            return languagesNames;
        }

        private string GetResourceFileNameWithoutExtension(string resourcePath)
        {
            var removeStartPath = resourcePath.Remove(0, GetResourcesAssemblyPath().Length + 1);
            var removeExtension = Path.GetFileNameWithoutExtension(removeStartPath);

            return removeExtension;
        }

        private void LoadLocalizableForCulture(CultureInfo culture)
        {
            _logger.LogInformation("Load localizable file");

            _currentCulture = culture;

            var currentLanguage = _currentCulture.Name;
            var languageFilename = $"{currentLanguage}.xml";
            var languageExists = ResourceExists(languageFilename);

            if (!languageExists)
            {
                //  Fallback on default language

                _logger.LogWarning($"Language <{languageFilename}> not found; Use fallback <{DefaultLanguageCode}.xml>");
                _currentCulture = new CultureInfo(DefaultLanguageCode);

                languageFilename = $"{DefaultLanguageCode}.xml";
            }

            var xmlSerializer = new XmlSerializer(typeof(Language));

            using (var stream = GetResourceStream(languageFilename))
            {
                var loadedLanguage = (Language)xmlSerializer.Deserialize(stream);

                _currentLanguage = loadedLanguage;
            }
        }

        public string GetResourcesAssemblyPath() => "Nemeio.Core.Resources.Languages";

        protected Assembly GetCoreAssembly() => typeof(CoreHelpers).Assembly;

        public virtual bool ResourceExists(string resourceName)
        {
            var resourceNames = GetCoreAssembly().GetManifestResourceNames();
            var fileName = $"{GetResourcesAssemblyPath()}.{resourceName}";

            return resourceNames.Contains(fileName);
        }

        public virtual Stream GetResourceStream(string filename)
        {
            return GetCoreAssembly().GetManifestResourceStream($"{GetResourcesAssemblyPath()}.{filename}");
        }

        public void InjectApplicationSettingsManager(IApplicationSettingsProvider applicationSettingsManager)
        {
            _applicationSettingsManager = applicationSettingsManager;
        }

        private void RaiseCultureChanged() => LanguageChanged?.Invoke(this, null);

        private void RaiseLanguageDueCare() => LanguageDueCare?.Invoke(this, null);
    }
}
