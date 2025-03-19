using System;
using System.IO;
using Nemeio.Core.Extensions;

namespace Nemeio.Core
{
    public static class NemeioConstants
    {
        public static readonly string KeyboardIp                    = "http://10.0.0.1/"; //Keyboard
        public static readonly string ServerUrl                     = "http://karmeliet.witekio.com/api";

        // can switch between versions and versions_v2 to ensure compatibility with last version
        public static readonly string UpdatesUrl                    = ServerUrl + "/updateInformation";

        public static readonly long VendorId                        = 0x0483;
        public static readonly long ProductIdWithInstaller          = 0x1234;
        public static readonly long ProductIdWithoutInstaller       = 0x1235;

        public static readonly int NemeioKeepAliveTimeout           = 1000;
        public static readonly int NemeioUpdaterTimeout             = 86400000; // 24h
        public static readonly int NemeioBatteryTimeout             = 60000;

        public static readonly int NemeioDefaultDelay               = 500;
        public static readonly int NemeioDefaultSpeed               = 40;

        public static readonly int NemeioMinimumBatteryLevel        = 10;
        public static readonly int NemeioLowLevelBattery            = 5;
        public static readonly int NemeioVeryLowLevelBattery        = 2;
        public static readonly ushort NemeioBatteryNotPlugged       = 0;

        public static readonly int NotificationTimeout              = 5000;

        public static readonly string AppName                       = "Nemeio";
        public static readonly string ManufacturerName              = "Nemeio";
        public static readonly string LogFolderName                 = "logs";
        public static readonly string LogFileName                   = "nemeio";
        public static readonly string KeyboardCrashFileName         = "kbcrash";
        public static readonly string LogExtension                  = ".log";
        public static readonly string MacInstallerExtension         = ".dmg";
        public static readonly string WinInstallerExtension         = ".exe";

        public static readonly string TemporaryDirectoryName        = "tmp";

        public static readonly int DefaultCategoryId                = 1;

        private static readonly string MacLibraryName               = "Library";
        private static readonly string MacApplicationSupportName    = "Application Support";

        public static readonly string DbFileName                    = "nemeio.db";
        public static readonly string PasswordFilename              = "nemeio";

        public static readonly string Noto                          = "NotoSans-Regular.ttf";
        public static readonly string Cairo                         = "Cairo-Regular.ttf";
        public static readonly string NotoJP                        = "NotoSansJP-Regular.otf";
        public static readonly string NotoKR                        = "NotoSansKR-Regular.otf";
        public static readonly string Roboto                        = "Roboto-Regular.ttf";

        public static string LogPath
        {
            get
            {
                if (ObjectExtensions.IsOSXPlatform())
                {
                    return Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                        MacLibraryName,
                        MacApplicationSupportName,
                        AppName,
                        LogFolderName
                    );
                }
                else
                {
                    return Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        AppName,
                        LogFolderName
                    );
                }
            }
        }
    }
}
