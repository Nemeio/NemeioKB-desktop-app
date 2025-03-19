using System;
using System.Collections.Generic;
using System.IO;
using Foundation;
using Nemeio.Core.Enums;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems.Keyboards;
using Nemeio.LayoutGen.Resources;
using Nemeio.Mac.Native;

namespace Nemeio.Platform.Mac.Keyboards
{
    public class MacKeyboardBuilder : ISystemKeyboardBuilder
    {
        public void ForEachModifiers(Action<KeyboardModifier> action)
        {
            var modifiers = new List<KeyboardModifier>() 
            { 
                KeyboardModifier.None, 
                KeyboardModifier.Shift, 
                KeyboardModifier.AltGr, 
                KeyboardModifier.Shift | KeyboardModifier.AltGr,
                KeyboardModifier.Capslock
            };

            foreach (var modifier in modifiers)
            {
                action(modifier);
            }
        }

        public KeyDisposition GetKeyDisposition(uint scanCode, bool isRequiredKey, bool isFunctionKey)
        {
            if (isRequiredKey && !isFunctionKey)
            {
                return KeyDisposition.Single;
            }

            if (isFunctionKey)
            {
                return KeyDisposition.Full;
            }

            return KeyDisposition.Double;
        }

        public string GetKeyValue(uint sc, KeyboardModifier modifier, OsLayoutId layout)
        {
            var hasShift = (modifier & KeyboardModifier.Shift) > 0;
            var hasAltGr = (modifier & KeyboardModifier.AltGr) > 0;
            var hasCaplock = (modifier & KeyboardModifier.Capslock) > 0;

            var strForKey = ExtendedTools.CreateStringForKeyWithModifiers((ushort)sc, new NSString(layout), hasShift, hasAltGr, hasCaplock);

            return !string.IsNullOrWhiteSpace(strForKey) ? strForKey : string.Empty;
        }

        public Stream LoadEmbeddedResource(string name) => Resources.GetResourceStream(name);

        /*private string ConvertToUpperIfNeeded(string res, uint sc, bool hasShift, bool hasAltGr)
        {
            if (Enum.IsDefined(typeof(LetterKeycode), sc) && !hasShift && !hasAltGr)
            {
                return res.ToUpper();
            }

            return res;
        }*/
    }
}
