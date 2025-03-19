using System;
using System.Configuration;

namespace Nemeio.WinAutoInstaller
{
    public static class SettingsHelper
    {
        public static string ReadSetting(string key)
        {
            string result = null;
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key] ?? null;
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine($"Error reading app settings <{key}>");
            }
            return result;
        }
    }
}
