using System.Threading;
using CoreFoundation;
using Foundation;
using Nemeio.Mac.ExtendedTools;

namespace Nemeio.Mac.Native
{
    public class ExtendedTools
    {
        private const string AlreadyRegisterAppAutoLaunch = "appAtLaunch";

        /// <summary>
        /// Testing when layout is generated and updated from keyboard side (back buttons ° and °°) we encountered complaints on following model
        /// <2019-10-02 09:13:36.736 Nemeio[49363:4940955] pid(49363)/euid(503) is calling TIS/TSM in non-main thread environment, ERROR : This is NOT allowed. Please call TIS/TSM in main thread!!!>
        /// We traced down that the above methods - prefixed as Extern now - were expected to be run on main thread.
        /// So we encapsulated them so as toensure they are appropriately called and this adequately supressed our TIS/TSM complaints

        public static ushort KeyCodeForChar(char c)
        {
            ushort result = 0;
            if (Thread.CurrentThread.IsBackground == true)
            {
                DispatchQueue.MainQueue.DispatchSync(() =>
                {
                    result = LayoutHelper.KeyCodeForChar(c);
                });
            }
            else
            {
                result = LayoutHelper.KeyCodeForChar(c);
            }
            return result;
        }

        public static NSString CreateStringForKeyWithModifiers(ushort keyCode, string layoutName, bool shift, bool altGr, bool capslock)
        {
            NSString result = null;
            var objCLayoutName = new NSString(layoutName);
            if (Thread.CurrentThread.IsBackground == true)
            {
                DispatchQueue.MainQueue.DispatchSync(() =>
                {
                    result = LayoutHelper.CreateStringForKeyWithModifiers(keyCode, objCLayoutName, shift, altGr, capslock);
                });
            }
            else
            {
                result = LayoutHelper.CreateStringForKeyWithModifiers(keyCode, objCLayoutName, shift, altGr, capslock);
            }
            return result;
        }

        public static bool SetCurrentKeyboardLayout(string layoutName)
        {
            bool result = false;
            var objCLayoutName = new NSString(layoutName);

            if (Thread.CurrentThread.IsBackground)
            {
                DispatchQueue.MainQueue.DispatchSync(() =>
                {
                    result = LayoutHelper.SetCurrentKeyboardLayout(objCLayoutName);
                });
            }
            else
            {
                result = LayoutHelper.SetCurrentKeyboardLayout(objCLayoutName);
            }

            return result;
        }

        public static void RegisterAppAtLaunchIfNeeded()
        {
            var userDefaults = NSUserDefaults.StandardUserDefaults;
            var alreadyDone = userDefaults.BoolForKey(AlreadyRegisterAppAutoLaunch);

            if (!alreadyDone)
            {
                LoginHelper.RegisterApplicationAtLogin();

                userDefaults.SetBool(true, AlreadyRegisterAppAutoLaunch);
                userDefaults.Synchronize();
            }
        }

        public static void IncreaseVolume(float amount) => AudioHelper.IncreaseSystemVolume(amount);

        public static void DecreaseVolume(float amount) => AudioHelper.DecreaseSystemVolume(amount);

        public static void ReverseMute()
        {
            var isMuted = AudioHelper.IsMuted();

            AudioHelper.ApplyMute(!isMuted);
        }
    }
}