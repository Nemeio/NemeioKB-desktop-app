using System;

using AppKit;
using Foundation;
using ObjCRuntime;
using CoreGraphics;

namespace Nemeio.Mac.ExtendedTools
{
    [DisableDefaultCtor]
    [BaseType(typeof(NSObject))]
    interface LayoutHelper
    {
        //  + (BOOL) setCurrentKeyboardLayout:(char*) layoutName;
        [Static]
        [Export("setCurrentKeyboardLayout:")]
        bool SetCurrentKeyboardLayout(NSString layoutName);

        //  + (NSString*) createStringForKeyWithModifiers: (CGKeyCode)keyCode withLayout: (NSString*) layoutName withShift: (BOOL)shift andWithAltGr: (BOOL)altGr capslock: (BOOL)capslock;
        [Static]
        [Export("createStringForKeyWithModifiers:withLayout:withShift:andWithAltGr:andCapslock:")]
        NSString CreateStringForKeyWithModifiers(ushort keyCode, NSString layoutName, bool withShift, bool andWithAltGr, bool capslock);

        //  + (NSString*) createStringForKey: (CGKeyCode)keyCode;
        [Static]
        [Export("createStringForKey:")]
        NSString CreateStringForKey(ushort keyCode);

        //  + (CGKeyCode) keyCodeForChar: (const UniChar)character
        [Static]
        [Export("keyCodeForChar:")]
        ushort KeyCodeForChar(char character);
    }

    [DisableDefaultCtor]
    [BaseType(typeof(NSObject))]
    interface LoginHelper
    {
        //  + (void)registerAppAtLogin;
        [Static]
        [Export("registerAppAtLogin")]
        void RegisterApplicationAtLogin();

        //  + (void)unregisterAppAtLogin;
        [Static]
        [Export("unregisterAppAtLogin")]
        void UnregisterApplicationAtLogin();
    }

    [DisableDefaultCtor]
    [BaseType(typeof(NSObject))]
    interface AudioHelper
    {
        //  + (void) setSystemVolume: (float) inVolume;
        [Static]
        [Export("setSystemVolume:")]
        void SetSystemVolume(float inVolume);

        //  + (void) increaseSystemVolume: (float) byAmount;
        [Static]
        [Export("increaseSystemVolume:")]
        void IncreaseSystemVolume(float byAmount);

        //  + (void) decreaseSystemVolume: (float) byAmount;
        [Static]
        [Export("decreaseSystemVolume:")]
        void DecreaseSystemVolume(float byAmount);

        //  + (void) applyMute: (BOOL) mute;
        [Static]
        [Export("applyMute:")]
        void ApplyMute(bool mute);

        //  + (BOOL) isMuted;
        [Static]
        [Export("isMuted")]
        bool IsMuted();
    }
}
