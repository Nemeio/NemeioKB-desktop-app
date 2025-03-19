using System.Linq;
using AppKit;
using Foundation;
using Microsoft.Extensions.Logging;
using MvvmCross.Mac.Platform;
using MvvmCross.Platform;
using Nemeio.Core.Applications;
using Nemeio.Core.Services;
using Nemeio.Presentation;

namespace Nemeio.Mac
{
    [Register("AppDelegate")]
    public partial class AppDelegate : MvxApplicationDelegate, INSUserNotificationCenterDelegate
    {
        private static string ApplicationBundleId = "com.witekio.karmeliet";

        private bool _setupDone;
        private MacNemeioAppSetup _setupMvvm;
        private ILoggerFactory _loggerFactory;
            
        public AppDelegate()
        {
            _loggerFactory = Logger.GetLoggerFactory();
            System.AppContext.SetSwitch("ISMACOS", true);
        }

        public override void WillBecomeActive(NSNotification notification) { }

        public override void DidResignActive(NSNotification notification) { }

        public override void DidFinishLaunching(NSNotification notification)
        {
            //  Only one instance is permitted on same time
            if (!IsUniqueInstance())
            {
                NSApplication.SharedApplication.Terminate(this);
            }

            _setupMvvm = new MacNemeioAppSetup(_loggerFactory, this);
            _setupMvvm.Initialize();
            _setupDone = true;

            Presentation.Application.ApplicationStarted();

            //  FIXME: The implementation of the "INSUserNotificationCenterDelegate" interface is only a test to fix the BLDLCK-2143 bug. 
            //  If this bug persists, please remove this code.
            //  According to the documentation :
            //  https://developer.apple.com/documentation/foundation/nsusernotificationcenterdelegate#//apple_ref/occ/intfm/NSUserNotificationCenterDelegate/userNotificationCenter:shouldPresentNotification
            //  Needed if we want to be sure all notification will be raised
            NSUserNotificationCenter.DefaultUserNotificationCenter.Delegate = this;

            Native.ExtendedTools.RegisterAppAtLaunchIfNeeded();

            NSApplication.SharedApplication.MainMenu = new NSMenu();
            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
        }

        public override void WillTerminate(NSNotification notification)
        {
            if (!_setupDone)
            {
                return;
            }

            Mvx.Resolve<IApplicationController>()?.ShutDown();
            Mvx.Resolve<INemeioHttpService>()?.StopListeningToRequestsAsync();
        }

        [Export("userNotificationCenter:shouldPresentNotification:")]
        public bool ShouldPresentNotification(NSUserNotificationCenter center, NSUserNotification notification)
        {
            //  We always want to show application notifications

            return true;
        }

        private bool IsUniqueInstance()
        {
            var runningApps = NSWorkspace.SharedWorkspace.RunningApplications;

            var numberOfRunningNemeioApp = runningApps.Count(application =>
            {
                if (application.BundleIdentifier != null)
                {
                    return application.BundleIdentifier.Equals(ApplicationBundleId);
                }

                return false;
            });

            return numberOfRunningNemeioApp == 1;
        }
    }
}
