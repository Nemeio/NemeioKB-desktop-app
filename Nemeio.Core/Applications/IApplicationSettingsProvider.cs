using System;
using System.Globalization;
using Nemeio.Core.Applications.Manifest;
using Nemeio.Core.Services.AppSettings;

namespace Nemeio.Core.Applications
{
    public interface IApplicationSettingsProvider
    {
        ApplicationSettings ApplicationSettings { get; }
        CultureInfo Language { get; set; }
        bool AugmentedImageEnable { get; set; }
        bool ShowGrantPrivilegeWindow { get; set; }
        Version UpdateTo { get; set; }
        string LastRollbackManifestString { get; set; }

        event EventHandler<EventArgs> LanguageChanged;
        event EventHandler<EventArgs> AugmentedImageEnableChanged;
        event EventHandler<EventArgs> ShowGrantPrivilegeWindowChanged;
        event EventHandler<EventArgs> UpdateToChanged;
        event EventHandler<EventArgs> LastRollbackManifestStringChanged;
    }
}
