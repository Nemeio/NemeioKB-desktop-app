using System;
using Foundation;
using UserNotifications;

namespace Nemeio.Mac.Services
{
    public class MacOSNotificationDelegate : NSObject, IUNUserNotificationCenterDelegate
    {
        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Sound);
        }
    }
}
