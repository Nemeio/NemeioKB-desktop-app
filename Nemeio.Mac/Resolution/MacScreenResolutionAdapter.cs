using System;
using Foundation;
using Nemeio.Presentation.Menu;

namespace Nemeio.Mac.Resolution
{
    public class MacScreenResolutionAdapter : NSObject, IScreenResolutionAdapter
    {
        private const string ScreenParametersNotificationName = "NSApplicationDidChangeScreenParametersNotification";

        public event EventHandler OnScreenResolutionChanged;

        public MacScreenResolutionAdapter()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(new NSString(ScreenParametersNotificationName), ScreenParametersChanged);
        }

        ~MacScreenResolutionAdapter()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(this);
        }

        public void ScreenParametersChanged(NSNotification notification)
        {
            OnScreenResolutionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
