using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Systems.Sessions;

namespace Nemeio.Platform.Mac.Sessions
{
    public class MacSystemSessionStateWatcher : NSObject, ISystemSessionStateWatcher
    {
        private const string SessionNotificationSelectorName = "handleSessionNotification:";
        private const string SessionIsLockedNotification = "com.apple.screenIsLocked";
        private const string SessionIsUnlockedNotification = "com.apple.screenIsUnlocked";

        private readonly ILogger _logger;

        private NSObject _lockObserver;
        private NSObject _unlockObserver;

        public event EventHandler<SessionStateChangedEventArgs> StateChanged;

        public MacSystemSessionStateWatcher(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MacSystemSessionStateWatcher>();

            _lockObserver = NSWorkspace.SharedWorkspace.NotificationCenter.AddObserver(NSWorkspace.SessionDidBecomeActiveNotification, null, NSOperationQueue.MainQueue, SessionDidBecomeActive);
            _unlockObserver = NSWorkspace.SharedWorkspace.NotificationCenter.AddObserver(NSWorkspace.SessionDidResignActiveNotification, null, NSOperationQueue.MainQueue, SessionDidResignActive);

            var distributionCenter = GetDefaultNotificationCenter();
            distributionCenter.AddObserver(this, new ObjCRuntime.Selector(SessionNotificationSelectorName), SessionIsLockedNotification, null, NSNotificationSuspensionBehavior.DeliverImmediately);
            distributionCenter.AddObserver(this, new ObjCRuntime.Selector(SessionNotificationSelectorName), SessionIsUnlockedNotification, null, NSNotificationSuspensionBehavior.DeliverImmediately);
        }

        protected override void Dispose(bool disposing)
        {
            NSWorkspace.SharedWorkspace.NotificationCenter.RemoveObserver(_lockObserver);
            NSWorkspace.SharedWorkspace.NotificationCenter.RemoveObserver(_unlockObserver);

            var distributionCenter = GetDefaultNotificationCenter();
            distributionCenter.RemoveObserver(this);

            base.Dispose(disposing);
        }

        [Export(SessionNotificationSelectorName)]
        public void HandleNotification(NSNotification notification)
        {
            if (notification.Name == null)
            {
                return;
            }

            switch (notification.Name)
            {
                case SessionIsLockedNotification:
                    RaiseSessionIsLocked();
                    break;
                case SessionIsUnlockedNotification:
                    RaiseSessionIsOpen();
                    break;
            }
        }

        private NSDistributedNotificationCenter GetDefaultNotificationCenter()
        {
            return (NSDistributedNotificationCenter)NSDistributedNotificationCenter.DefaultCenter;
        }

        private void SessionDidBecomeActive(NSNotification obj)
        {
            RaiseSessionIsOpen();
        }

        private void SessionDidResignActive(NSNotification obj)
        {
            RaiseSessionIsLocked();
        }

        private void RaiseSessionIsOpen() => RaiseSessionEvent(SessionState.Open);

        private void RaiseSessionIsLocked() => RaiseSessionEvent(SessionState.Lock);

        private void RaiseSessionEvent(SessionState state)
        {
            //  Raise on another Thread
            Task.Run(() =>
            {
                StateChanged?.Invoke(this, new SessionStateChangedEventArgs(state));
            });
        }
    }
}
