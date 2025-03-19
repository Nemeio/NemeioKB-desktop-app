using Foundation;
using Nemeio.Core.Theme;

namespace Nemeio.Mac.Theme
{
    public class MacSystemThemeProvider : ISystemThemeProvider
    {
        private const string DarkThemeName = "Dark";
        private const string LightThemeName = "Light";

        public SystemTheme GetSystemTheme()
        {
            var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");
            if (string.IsNullOrEmpty(interfaceStyle))
            {
                //  By default MacOS is on light mode
                //  So we consider is the default value if we can't get current mode

                return SystemTheme.Light;
            }

            return interfaceStyle.Equals(DarkThemeName) ? SystemTheme.Dark : SystemTheme.Light;
        }
    }
}
