using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace Nemeio.Mac.Native
{
    using CGEventRef = System.IntPtr;
    using CGEventSourceRef = System.IntPtr;

    /* keycodes for keys that are independent of keyboard layout*/
    public enum CGKeyCode : ushort
    {
        kVK_Return              = 0x24,
        kVK_Tab                 = 0x30,
        kVK_Space               = 0x31,
        kVK_Delete              = 0x33,
        kVK_Escape              = 0x35,
        kVK_Command             = 0x37,
        kVK_Shift               = 0x38,
        kVK_CapsLock            = 0x39,
        kVK_Option              = 0x3A,
        kVK_Control             = 0x3B,
        kVK_RightCommand        = 0x36,
        kVK_RightShift          = 0x3C,
        kVK_RightOption         = 0x3D,
        kVK_RightControl        = 0x3E,
        kVK_Function            = 0x3F,
        kVK_F17                 = 0x40,
        kVK_VolumeUp            = 0x48,
        kVK_VolumeDown          = 0x49,
        kVK_Mute                = 0x4A,
        kVK_F18                 = 0x4F,
        kVK_F19                 = 0x50,
        kVK_F20                 = 0x5A,
        kVK_F5                  = 0x60,
        kVK_F6                  = 0x61,
        kVK_F7                  = 0x62,
        kVK_F3                  = 0x63,
        kVK_F8                  = 0x64,
        kVK_F9                  = 0x65,
        kVK_F11                 = 0x67,
        kVK_F13                 = 0x69,
        kVK_F16                 = 0x6A,
        kVK_F14                 = 0x6B,
        kVK_F10                 = 0x6D,
        kVK_F12                 = 0x6F,
        kVK_F15                 = 0x71,
        kVK_Help                = 0x72,
        kVK_Home                = 0x73,
        kVK_PageUp              = 0x74,
        kVK_ForwardDelete       = 0x75,
        kVK_F4                  = 0x76,
        kVK_End                 = 0x77,
        kVK_F2                  = 0x78,
        kVK_PageDown            = 0x79,
        kVK_F1                  = 0x7A,
        kVK_LeftArrow           = 0x7B,
        kVK_RightArrow          = 0x7C,
        kVK_DownArrow           = 0x7D,
        kVK_UpArrow             = 0x7E,
        kVK_None                = 0x00,
        kVK_Insert              = 0x72,
        kVK_Screenshot          = 0x44
    };

    public class QuartzEvent
    {
        private const uint kCGNullWindowID              = 0;
        private const string kCGWindowName              = "kCGWindowName";
        private const string kCGWindowOwnerPID          = "kCGWindowOwnerPID";
        private const string quartzServices             = "/System/Library/Frameworks/Quartz.framework/Quartz";

        //CGEventRef CGEventCreateKeyboardEvent(CGEventSourceRef source, CGKeyCode virtualKey, bool keyDown);
        [DllImport(quartzServices)]
        public static extern CGEventRef CGEventCreateKeyboardEvent(CGEventSourceRef source, CGKeyCode virtualKey, bool keyDown);

        //void CGEventPost(CGEventTapLocation tap, CGEventRef event);
        [DllImport(quartzServices)]
        public static extern void CGEventPost(CGEventTapLocation tap, CGEventRef ev);

        //void CGEventKeyboardSetUnicodeString(CGEventRef event, UniCharCount stringLength, const UniChar *unicodeString);
        [DllImport(quartzServices, CharSet = CharSet.Unicode)]
        public static extern void CGEventKeyboardSetUnicodeString(CGEventRef ev, int stringLength, char[] ar);

        //void CGEventSetFlags(CGEventRef event, CGEventFlags flags);
        [DllImport(quartzServices)]
        public static extern void CGEventSetFlags(CGEventRef ev, CGEventFlags flags);

        [DllImport(@"/System/Library/Frameworks/QuartzCore.framework/QuartzCore")]
        static extern CGEventRef CGWindowListCopyWindowInfo(CGWindowListOption option, uint relativeToWindow);

        public static string GetWindowInformation(int processIdentifier)
        {
            CGEventRef windowInfo = CGWindowListCopyWindowInfo(CGWindowListOption.All, kCGNullWindowID);
            if (windowInfo != CGEventRef.Zero)
            {
                NSArray values = Runtime.GetNSObject<NSArray>(windowInfo);
                if (values != null)
                {
                    for (nuint i = 0, len = values.Count; i < len; i++)
                    {
                        NSObject window = Runtime.GetNSObject(values.ValueAt(i));

                        var key = new NSString(kCGWindowOwnerPID);
                        var windowName = (NSString)window.ValueForKey(new NSString(kCGWindowName));
                        var currentOwnerPid = (NSNumber)window.ValueForKey(key);

                        if ((int)currentOwnerPid == processIdentifier)
                        {
                            return windowName;
                        }
                    }
                }
            }

            return string.Empty;
        }
    }
}
