using System;
using System.Linq;
using System.Runtime.InteropServices;
using AppKit;

namespace Nemeio.Mac.Native
{
    using CFStringRef = IntPtr;

    public class ServiceManagement
    {
        private const string serviceManagementLib = "/System/Library/Frameworks/ServiceManagement.framework/ServiceManagement";

        private const string launcherId = "com.ldlc.NemeioLauncher";

        //Boolean SMLoginItemSetEnabled(CFStringRef identifier, Boolean enabled);
        [DllImport(serviceManagementLib)]
        internal static extern bool SMLoginItemSetEnabled(CFStringRef identifier, bool enabled);

        public static bool StartAtLogin(bool value)
        {
            CoreFoundation.CFString id = new CoreFoundation.CFString(launcherId);
            bool result = SMLoginItemSetEnabled(id.Handle, value);
            return result;
        }

        public static bool LauncherIsRunning()
        {
            var runningsApps = NSWorkspace.SharedWorkspace.RunningApplications;
            return runningsApps.Any(a => a.BundleIdentifier == launcherId);
        }
    }
}
