using System;
using Microsoft.Win32;
using Nemeio.Presentation.Menu;

namespace Nemeio.Wpf.Resolution
{
    public class WinScreenResolutionAdapter : IScreenResolutionAdapter
    {
        public event EventHandler OnScreenResolutionChanged;

        public WinScreenResolutionAdapter()
        {
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        ~WinScreenResolutionAdapter()
        {
            SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            OnScreenResolutionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
