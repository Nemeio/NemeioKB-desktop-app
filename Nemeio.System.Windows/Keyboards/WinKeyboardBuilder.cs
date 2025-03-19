using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Nemeio.Core.Enums;
using Nemeio.Core.Resources;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems.Keyboards;
using Nemeio.Platform.Windows.Layouts;
using Nemeio.Windows.Win32;

namespace Nemeio.Platform.Windows.Keyboards
{
    public class WinKeyboardBuilder : ISystemKeyboardBuilder
    {
        private const int IsEnable = 0xff;
        private const int KeyboardStateLength = 256;

        private readonly IResourceLoader _resourceLoader;
        private readonly WinOsLayoutIdBuilder _osLayoutIdBuilder;

        public WinKeyboardBuilder(IResourceLoader resourceLoader, WinOsLayoutIdBuilder osLayoutIdBuilder)
        {
            _resourceLoader = resourceLoader ?? throw new ArgumentNullException(nameof(resourceLoader));
            _osLayoutIdBuilder = osLayoutIdBuilder ?? throw new ArgumentNullException(nameof(osLayoutIdBuilder));
        }

        public void ForEachModifiers(Action<KeyboardModifier> action)
        {
            var modifiers = new List<KeyboardModifier>()
            {
                KeyboardModifier.None,
                KeyboardModifier.Shift,
                KeyboardModifier.AltGr,
                KeyboardModifier.Shift | KeyboardModifier.AltGr,
                KeyboardModifier.Capslock,
                KeyboardModifier.Function
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

            return KeyDisposition.Full;
        }

        public string GetKeyValue(uint sc, KeyboardModifier modifier, OsLayoutId layout)
        {
            var winOsLayoutId = _osLayoutIdBuilder.Parse(layout);

            var vk = WinUser32.MapVirtualKeyExW(sc, (uint)WinUser32.MAPVK.MAPVK_VSC_TO_VK_EX, winOsLayoutId);
            var strForKey = GetCharsFromKeys(vk, modifier, layout);

            return !string.IsNullOrEmpty(strForKey) ? strForKey : string.Empty;
        }

        public Stream LoadEmbeddedResource(string name) => _resourceLoader.GetResourceStream(name);

        private string GetCharsFromKeys(uint keys, KeyboardModifier modifier, OsLayoutId layout)
        {
            var winLayout = _osLayoutIdBuilder.Parse(layout);
            var buf = new StringBuilder(KeyboardStateLength);
            var keyboardState = new byte[KeyboardStateLength];

            if ((modifier & KeyboardModifier.Shift) > 0)
            {
                keyboardState[(int)Keys.ShiftKey] = IsEnable;
            }

            if ((modifier & KeyboardModifier.AltGr) > 0)
            {
                keyboardState[(int)Keys.ControlKey] = IsEnable;
                keyboardState[(int)Keys.Menu] = IsEnable;
            }

            if ((modifier & KeyboardModifier.Capslock) > 0)
            {
                keyboardState[(int)Keys.CapsLock] = IsEnable;
            }

            WinUser32.ToUnicodeEx(keys, 0, keyboardState, buf, KeyboardStateLength, 0, winLayout);

            var result = buf.ToString();

            return result.Length > 1 ? result.Substring(result.Length - 1, 1) : result;
        }
    }
}
