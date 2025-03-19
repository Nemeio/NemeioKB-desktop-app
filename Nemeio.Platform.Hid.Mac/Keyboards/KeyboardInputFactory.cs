using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Nemeio.Core;
using Nemeio.Mac.Native;
using Nemeio.Platform.Mac;
using Nemeio.Platform.Mac.Utils;

namespace Nemeio.Platform.Hid.Mac.Keyboards
{
    internal class FlagMapItem
    {
        public string Literal { get; set; }

        public CGKeyCode KeyCode { get; set; }

        public CGEventFlags Flag { get; set; }
    }

    public class KeyboardInputFactory
    {
        /// <summary>
        /// Documentation : https://developer.apple.com/documentation/coregraphics/cgeventflags?language=objc
        /// </summary>
        private List<FlagMapItem> _flagsMap = new List<FlagMapItem>()
        {
            new FlagMapItem() { Literal = KeyboardLiterals.CMD,         KeyCode = CGKeyCode.kVK_Command,        Flag = CGEventFlags.Command },
            new FlagMapItem() { Literal = KeyboardLiterals.Shift,       KeyCode = CGKeyCode.kVK_Shift,          Flag = CGEventFlags.Shift },
            new FlagMapItem() { Literal = KeyboardLiterals.Shift,       KeyCode = CGKeyCode.kVK_RightShift,     Flag = CGEventFlags.Shift },
            new FlagMapItem() { Literal = KeyboardLiterals.Ctrl,        KeyCode = CGKeyCode.kVK_Control,        Flag = CGEventFlags.Control },
            new FlagMapItem() { Literal = KeyboardLiterals.Fn,          KeyCode = CGKeyCode.kVK_Function,       Flag = CGEventFlags.SecondaryFn },
            new FlagMapItem() { Literal = KeyboardLiterals.CapsLock,    KeyCode = CGKeyCode.kVK_CapsLock,       Flag = CGEventFlags.AlphaShift },
            new FlagMapItem() { Literal = KeyboardLiterals.Alt,         KeyCode = CGKeyCode.kVK_Option,         Flag = CGEventFlags.Alternate },
            new FlagMapItem() { Literal = KeyboardLiterals.AltGr,       KeyCode = CGKeyCode.kVK_RightOption,    Flag = CGEventFlags.Alternate },
        };

        public IntPtr CreateNewInput(List<string> keyPressed, string key, bool pressed)
        {
            var keyCode = MacKeyboardConstants.GetVirtualKey(key);

            return CreateNewInput(keyPressed, keyCode, pressed);
        }

        public IntPtr CreateNewInput(List<string> keyPressed, CGKeyCode keyCode, bool pressed)
        {
            if (_flagsMap.Select(x => x.KeyCode).Any(x => x == keyCode))
            {
                return IntPtr.Zero;
            }

            IntPtr newEvent = IntPtr.Zero;

            DispatchQueueUtils.DispatchSyncOnMainQueueIfNeeded(() =>
            {
                var source = new CGEventSource(CGEventSourceStateID.HidSystem);
                newEvent = QuartzEvent.CGEventCreateKeyboardEvent(source.Handle, keyCode, pressed);

                AddModifierFlags(keyPressed, keyCode, newEvent);
            });

            return newEvent;
        }

        public void AddModifierFlags(List<string> keyPressed, CGKeyCode keyCode, IntPtr inputEvent)
        {
            CGEventFlags flags = 0;

            foreach (var key in keyPressed)
            {
                if (_flagsMap.Select(x => x.Literal).Any(x => x == key))
                {
                    var currentFlagItem = _flagsMap.First(x => x.Literal == key);

                    if (currentFlagItem.KeyCode == keyCode)
                    {
                        //  Bypass when key is myself

                        return;
                    }

                    flags = flags | currentFlagItem.Flag;
                }
            }

            if (flags == 0)
            {
                return;
            }

            QuartzEvent.CGEventSetFlags(inputEvent, flags);
        }
    }
}
