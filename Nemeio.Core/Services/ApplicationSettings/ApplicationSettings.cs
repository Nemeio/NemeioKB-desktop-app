using System;
using System.Globalization;

namespace Nemeio.Core.Services.AppSettings
{
    public class ApplicationSettings
    {
        public CultureInfo Language { get; set; }
        public bool AugmentedImageEnable { get; set; }
        public bool ShowGrantPrivilegeWindow { get; set; }
        public Version UpdateTo { get; set; }
        public string LastRollbackManifestString { get; set; }

        public ApplicationSettings(CultureInfo language, bool augmentedImageEnable, bool showGrantPrivilegeWindow, Version updateTo, string lastRollbackManifestString)
        {
            Language = language;
            AugmentedImageEnable = augmentedImageEnable;
            ShowGrantPrivilegeWindow = showGrantPrivilegeWindow;
            UpdateTo = updateTo;
            LastRollbackManifestString = lastRollbackManifestString;
        }
    }
}
