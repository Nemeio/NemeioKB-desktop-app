using System;
using System.Collections.Generic;
using System.Globalization;
using Nemeio.Core.Applications;
using Nemeio.Core.DataModels.Locale;

namespace Nemeio.Core.Managers
{
    public interface ILanguageManager
    {
        event EventHandler<EventArgs> LanguageChanged;

        event EventHandler<EventArgs> LanguageDueCare;

        void Start();

        void Stop();

        CultureInfo GetCurrentCultureInfo();

        string GetLocalizedValue(StringId key);

        void SetCurrentCultureInfo(CultureInfo culture);

        void InjectApplicationSettingsManager(IApplicationSettingsProvider applicationSettingsManager);

        IEnumerable<CultureInfo> GetSupportedLanguages();

        void SelectMainLanguage(CultureInfo culture);
    }
}
