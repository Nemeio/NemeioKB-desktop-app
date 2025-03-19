using System.Collections.Generic;
using System.Linq;
using static Nemeio.Core.KeyboardLiterals;

namespace Nemeio.Platform.Windows
{
    public static class WinKeyboardConstants
    {
        public static bool IsModifierKey(string modifier) => _modifierKeys.Contains(modifier);

        public static bool IsExtendedKey(ushort vk) => _extendedKeys.Contains(vk);

        public static bool IsSpecialKey(string modifier) => _specialKeys.Contains(modifier);

        public static ushort GetVirtualKey(string keyboardCode) => _virtualKeyMappings.TryGetValue(keyboardCode, out var vKey) ? vKey : (ushort)0x00;

        private static readonly string[] _modifierKeys = {
            Ctrl,
            Alt,
            AltGr,
            Shift
        };

        private static readonly ushort[] _extendedKeys = {
            0x25,
            0x26,
            0x27,
            0x28
        };

        private static readonly Dictionary<string, ushort> _virtualKeyMappings = new Dictionary<string, ushort>
        {
            { VolumeUp,      0xAF },
            { VolumeDown,    0xAE },
            { VolumeMute,    0xAD },
            { Ctrl,          0x11 },
            { Alt,           0x12 },
            { Shift,         0x10 },
            { CapsLock,      0x14 },
            { Backspace,     0x08 },
            { Delete,        0x2E },
            { WindowsKey,    0x5B },
            { CMD,           0x5B },
            { Enter,         0x0D },
            { Escape,        0x1B },
            { Tab,           0x09 },
            { Ins,           0x2D },
            { PrintScreen,   0x2C },
            { F1,            0x70 },
            { F2,            0x71 },
            { F3,            0x72 },
            { F4,            0x73 },
            { F5,            0x74 },
            { F6,            0x75 },
            { F7,            0x76 },
            { F8,            0x77 },
            { F9,            0x78 },
            { F10,           0x79 },
            { F11,           0x7A },
            { F12,           0x7B },
            { ArrowUp,       0x26 },
            { ArrowDown,     0x28 },
            { ArrowLeft,     0x25 },
            { ArrowRight,    0x27 },
            { Search,        0xAA },
            { End,           0x23 },
            { LightLow,      0xAA },
            { LightUp,       0x6F },
            { StandBy,       0x005F },
            { Start,         0x24 },
            { Menu,          0x5D }
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
            Search,
            End,        // OK
            LightLow,
            LightUp,
            StandBy,
            Start,
            Menu
        };

    }
}
