using System.Collections.Generic;
using System.Linq;
using Nemeio.Mac.Native;
using static Nemeio.Core.KeyboardLiterals;

namespace Nemeio.Platform.Mac
{
    public static class MacKeyboardConstants
    {
        public static bool IsModifierKey(string key) => _modifierKeys.Contains(key);

        public static bool IsSpecialKey(string modifier) => _specialKeys.Contains(modifier);

        public static CGKeyCode GetVirtualKey(string key) => _virtualKeyMappings.TryGetValue(key, out CGKeyCode code) ? code : CGKeyCode.kVK_None;

        private static readonly List<string> _modifierKeys = new List<string>
        {
            Alt,
            Shift,
            Ctrl,
            AltGr,
            CMD,
            Fn
        };

        private static readonly Dictionary<string, CGKeyCode> _virtualKeyMappings = new Dictionary<string, CGKeyCode>()
        {
            { VolumeUp,         CGKeyCode.kVK_VolumeUp },
            { VolumeDown,       CGKeyCode.kVK_VolumeDown },
            { VolumeMute,       CGKeyCode.kVK_Mute },
            { AltGr,            CGKeyCode.kVK_RightOption },
            { Backspace,        CGKeyCode.kVK_Delete },
            { CMD,              CGKeyCode.kVK_Command },
            { WindowsKey,       CGKeyCode.kVK_Command },
            { Ctrl,             CGKeyCode.kVK_Control },
            { CapsLock,         CGKeyCode.kVK_CapsLock },
            { Shift,            CGKeyCode.kVK_Shift },
            { Space,            CGKeyCode.kVK_Space },
            { Delete,           CGKeyCode.kVK_ForwardDelete },
            { Enter,            CGKeyCode.kVK_Return },
            { Escape,           CGKeyCode.kVK_Escape },
            { Tab,              CGKeyCode.kVK_Tab },
            { Fn,               CGKeyCode.kVK_Function },
            { F1,               CGKeyCode.kVK_F1 },
            { F2,               CGKeyCode.kVK_F2 },
            { F3,               CGKeyCode.kVK_F3 },
            { F4,               CGKeyCode.kVK_F4 },
            { F5,               CGKeyCode.kVK_F5 },
            { F6,               CGKeyCode.kVK_F6 },
            { F7,               CGKeyCode.kVK_F7 },
            { F8,               CGKeyCode.kVK_F8 },
            { F9,               CGKeyCode.kVK_F9 },
            { F10,              CGKeyCode.kVK_F10 },
            { F11,              CGKeyCode.kVK_F11 },
            { F12,              CGKeyCode.kVK_F12 },
            { ArrowUp,          CGKeyCode.kVK_UpArrow },
            { ArrowDown,        CGKeyCode.kVK_DownArrow },
            { ArrowLeft,        CGKeyCode.kVK_LeftArrow },
            { ArrowRight,       CGKeyCode.kVK_RightArrow },
            { Alt,              CGKeyCode.kVK_Option },
            { Start,            CGKeyCode.kVK_Home },
            { End,              CGKeyCode.kVK_End },
            { LightLow,         CGKeyCode.kVK_F14 },
            { LightUp,          CGKeyCode.kVK_F15 },
            { PrintScreen,      CGKeyCode.kVK_Screenshot },
            { StandBy,          CGKeyCode.kVK_None },
            { Search,           CGKeyCode.kVK_None }
        };

        private static readonly string[] _specialKeys = {
           Ctrl,
           Alt,
           AltGr,
           Shift,
           VolumeUp,
           VolumeDown,
           VolumeMute,
           CapsLock,
           Backspace,
           Delete,
           WindowsKey,
           CMD,
           Enter,
           Escape,
           Tab,
           Ins,
           PrintScreen,
           F1,
           F2,
           F3,
           F4,
           F5,
           F6,
           F7,
           F8,
           F9,
           F10,
           F11,
           F12,
           ArrowUp,
           ArrowDown,
           ArrowLeft,
           ArrowRight,
           Start,
           End,
           Search,
           LightLow,
           LightUp,
           StandBy,
           Fn
       };
    }
}
