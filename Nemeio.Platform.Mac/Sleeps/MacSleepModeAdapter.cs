using System;
using AppKit;
using Foundation;
using Nemeio.Core.Systems.Sleep;

namespace Nemeio.Platform.Mac.Sleeps
{
    public sealed class MacSleepModeAdapter : NSObject, ISleepModeAdapter
    {
        private NSObject _wakeObserver;
        private NSObject _sleepObserver;

        public event EventHandler<SleepModeChangedEventArgs> OnSleepModeChanged;

        public MacSleepModeAdapter()
        {
            _wakeObserver = NSWorkspace.SharedWorkspace.NotificationCenter.AddObserver(NSWorkspace.DidWakeNotification, null, NSOperationQueue.MainQueue, SystemWakeUp);
            _sleepObserver = NSWorkspace.SharedWorkspace.NotificationCenter.AddObserver(NSWorkspace.WillSleepNotification, null, NSOperationQueue.MainQueue, SystemWillSleep);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            NSWorkspace.SharedWorkspace.NotificationCenter.RemoveObserver(_wakeObserver);
            NSWorkspace.SharedWorkspace.NotificationCenter.RemoveObserver(_sleepObserver);
        }

        private void SystemWakeUp(NSNotification obj) => OnSleepModeChanged?.Invoke(this, new SleepModeChangedEventArgs(SleepMode.Awake));

        private void SystemWillSleep(NSNotification obj) => OnSleepModeChanged?.Invoke(this, new SleepModeChangedEventArgs(SleepMode.Sleep));
    }
}
