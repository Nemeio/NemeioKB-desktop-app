using System;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Nemeio.Core.Theme;

namespace Nemeio.Wpf.Theme
{
    public class WinSystemThemeProvider : ISystemThemeProvider
    {
        private readonly ILogger _logger;

        private const int LightMode = 1;
        private const int DarkMode = 0;

        public WinSystemThemeProvider(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<WinSystemThemeProvider>();
        }

        /// <summary>
        /// This method retrieves the current system theme. 
        /// It has been designed for Windows 10. 
        /// However, it uses the Windows registry. 
        /// In case of error it will return the default value "Dark".
        /// </summary>
        /// <returns></returns>
        public SystemTheme GetSystemTheme()
        {
            try
            {
                const string keyName = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
                const string valueName = "SystemUsesLightTheme";

                const int defaultValue = 1;

                //  Return 1 for Light
                //  Return 0 for Dark
                var appsUseLightTheme = (int)Registry.GetValue(keyName, valueName, defaultValue);

                return appsUseLightTheme == LightMode ? SystemTheme.Light : SystemTheme.Dark;
            }
            catch (Exception exception)
            {
                _logger.LogError("Impossible to get system theme on Windows", exception);

                //  By default Windows 10 is in dark mode
                //  So we consider that it's the default value

                return SystemTheme.Dark;
            }
        }
    }
}
