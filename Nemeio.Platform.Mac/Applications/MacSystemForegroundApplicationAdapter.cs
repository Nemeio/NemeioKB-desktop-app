using System;
using AppKit;
using Foundation;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Systems.Applications;
using Nemeio.Mac.Native;

namespace Nemeio.Platform.Mac.Applications
{
    public class MacSystemForegroundApplicationAdapter : ISystemForegroundApplicationAdapter
    {
        private const string ApplicationPathPrefix = "file://";
        private const string ApplicationKey = "NSWorkspaceApplicationKey";

        private ILogger _logger;
        private NSObject _notificationLaunchApp;
        private NSObject _notificationActivateApp;

        public MacSystemForegroundApplicationAdapter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MacSystemForegroundApplicationAdapter>();
        }

        public event EventHandler<ApplicationChangedEventArgs> OnApplicationChanged;

        public void Start()
        {
            _notificationLaunchApp = NSWorkspace.SharedWorkspace.NotificationCenter.AddObserver(NSWorkspace.DidLaunchApplicationNotification, null, NSOperationQueue.MainQueue, ActivateApplicationNotification);
            _notificationActivateApp = NSWorkspace.SharedWorkspace.NotificationCenter.AddObserver(NSWorkspace.DidActivateApplicationNotification, null, NSOperationQueue.MainQueue, ActivateApplicationNotification);
        }

        public void Stop()
        {
            //  LEGACY
            //  FIXME/TODO Create crash when called
            //  NSWorkspace.SharedWorkspace.NotificationCenter.RemoveObserver(_notificationLaunchApp, NSWorkspace.DidLaunchApplicationNotification);
            //  NSWorkspace.SharedWorkspace.NotificationCenter.RemoveObserver(_notificationActivateApp, NSWorkspace.DidActivateApplicationNotification);
        }

        private void ActivateApplicationNotification(NSNotification obj)
        {
            var application = obj.UserInfo[ApplicationKey] as NSRunningApplication;
            var path = Uri.UnescapeDataString(application.BundleUrl.AbsoluteString);

            if (path.StartsWith(ApplicationPathPrefix, StringComparison.CurrentCulture))
            {
                var subString = path.Substring(ApplicationPathPrefix.Length);
                path = subString.TrimEnd('/');
            }

            var windowTitle = QuartzEvent.GetWindowInformation(application.ProcessIdentifier);

            var app = new Application()
            {
                ApplicationPath = path,
                Name = application.LocalizedName,
                WindowTitle = windowTitle,
                ProcessId = application.ProcessIdentifier,
                ProcessName = application.LocalizedName,
            };

            _logger.LogTrace($"MacForegroundAppWatcher.DidLaunchApplicationNotification <Path={app.ApplicationPath}> <ApplicationName={app.Name}>");

            OnApplicationChanged?.Invoke(this, new ApplicationChangedEventArgs(app));
        }
    }
}
