using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core;
using Nemeio.Platform.Windows;
using Nemeio.Windows.Win32;

namespace Nemeio.Platform.Hid.Windows.Keyboards
{
    public class VirtualKeyBuilder
    {
        private static List<string> ModifiersList = new List<string>()
        {
            KeyboardLiterals.Shift,
            KeyboardLiterals.Ctrl,
            KeyboardLiterals.Alt
        };

        protected IntPtr _keyboardLayout;
        protected List<WinUser32.INPUT> _inputDownList;
        protected List<WinUser32.INPUT> _inputUpList;

        public VirtualKeyBuilder() => Reset(IntPtr.Zero);

        public static WinUser32.INPUT CreateInput(string key, bool pressed, IntPtr keyboardLayout)
        {
            var vkOfChar = WinKeyboardConstants.GetVirtualKey(key);
            var input = !pressed ? WinUser32.NewUpInput(vkOfChar, keyboardLayout) : WinUser32.NewDownInput(vkOfChar, keyboardLayout);

            if (WinKeyboardConstants.IsExtendedKey(vkOfChar))
            {
                SetExtendedKey(input);
            }

            return input;
        }

        public static void SetExtendedKey(WinUser32.INPUT input) => input.union.keyboardInput.flags |= WinUser32.KEYEVENTF_EXTENDEDKEY;

        public IList<WinUser32.INPUT> Build() => _inputDownList.Concat(_inputUpList).ToList();

        public void Reset(IntPtr keyboardLayout)
        {
            _keyboardLayout = keyboardLayout;
            _inputDownList = new List<WinUser32.INPUT>();
            _inputUpList = new List<WinUser32.INPUT>();
        }

        public void AddKey(string key, bool pressed)
        {
            var specialInput = CreateInput(key, pressed, _keyboardLayout);

            AddToList(specialInput, pressed);
        }

        public void AddUnicodeKey(char unicodeChar, bool pressed)
        {
            var input = pressed ? WinUser32.NewUnicodeDownInput(unicodeChar) : WinUser32.NewUnicodeUpInput(unicodeChar);

            AddToList(input, pressed);
        }

        public void AddVirtualKey(char character, ushort virtualKey)
        {
            if (VirtualKeyIsKnown(virtualKey))
            {
                int modifiers = virtualKey >> 8;

                AddPressedModifierListIfNeeded(modifiers, ModifiersList);

                _inputDownList.Add(
                    WinUser32.NewDownInput(virtualKey, _keyboardLayout)
                );

                AddUnpressedModifierListIfNeeded(modifiers, ModifiersList);
            }
            else
            {
                AddUnicodeKey(character, true);
            }
        }

        private bool VirtualKeyIsKnown(ushort virtualKey) => virtualKey != default(ushort);

        private void AddToList(WinUser32.INPUT input, bool pressed)
        {
            if (pressed)
            {
                _inputDownList.Add(input);
            }
            else
            {
                _inputUpList.Add(input);
            }
        }

        private void AddModifierIfNeeded(int virtualKeyModifiers, string keyLiteral, bool pressed)
        {
            int modifier = -1;

            switch (keyLiteral)
            {
                case KeyboardLiterals.Shift:
                    modifier = 1;
                    break;
                case KeyboardLiterals.Alt:
                    modifier = 4;
                    break;
                case KeyboardLiterals.Ctrl:
                    modifier = 2;
                    break;
                default:
                    throw new NotSupportedException("Invalid KeyLiteral used");
            }

            if ((virtualKeyModifiers & modifier) != 0)
            {
                AddKey(keyLiteral, pressed);
            }
        }

        private void AddPressedModifierIfNeeded(int virtualKeyModifiers, string keyLiteral)
        {
            AddModifierIfNeeded(virtualKeyModifiers, keyLiteral, true);
        }

        private void AddUnpressedModifierIfNeeded(int virtualKeyModifiers, string keyLiteral)
        {
            AddModifierIfNeeded(virtualKeyModifiers, keyLiteral, false);
        }

        private void AddPressedModifierListIfNeeded(int virtualKeyModifiers, List<string> keyLiterals)
        {
            foreach (var keyLiteral in keyLiterals)
            {
                AddPressedModifierIfNeeded(virtualKeyModifiers, keyLiteral);
            }
        }

        private void AddUnpressedModifierListIfNeeded(int virtualKeyModifiers, List<string> keyLiterals)
        {
            foreach (var keyLiteral in keyLiterals)
            {
                AddUnpressedModifierIfNeeded(virtualKeyModifiers, keyLiteral);
            }
        }
    }
}
