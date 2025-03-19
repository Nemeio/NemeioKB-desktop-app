using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Win32;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Managers;
using Nemeio.Core.Services.Layouts;
using Nemeio.Windows.Win32;

namespace Nemeio.Platform.Windows.Layouts
{
    public class WinOsLayoutIdBuilder
    {
        private static int CultureMask = 0x0000FFFF;

        private readonly ILanguageManager _languageManager;
        private readonly IDictionary<string, string> _layoutNames;

        public WinOsLayoutIdBuilder(ILanguageManager languageManager)
        {
            _languageManager = languageManager ?? throw new ArgumentNullException(nameof(languageManager));
            _layoutNames = new Dictionary<string, string>();

            ConstructLayoutNameList();
        }

        public WinOsLayoutId Build(IntPtr baseLayoutHandle)
        {
            if (baseLayoutHandle == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(baseLayoutHandle));
            }

            var name = ComputeLayoutName(baseLayoutHandle);
            var osLayoutId = new WinOsLayoutId(name, baseLayoutHandle);

            return osLayoutId;
        }

        public WinOsLayoutId Parse(OsLayoutId osLayoutId)
        {
            var systemHandle = new IntPtr(Convert.ToInt32(osLayoutId.Id));
            var layout = Build(systemHandle);

            return layout;
        }

        private string ComputeLayoutName(IntPtr baseLayoutHandle)
        {
            if (baseLayoutHandle == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(baseLayoutHandle));
            }

            var language = GetLanguageCulture(baseLayoutHandle);
            var keyboard = GetKeyboardLayoutName(baseLayoutHandle) ?? GetKeyboardName(baseLayoutHandle);

            var languageName = language != null ? language.DisplayName : "???";
            var keyboardName = keyboard ?? "Unknown";

            var layoutName = $"{languageName}\n{_languageManager.GetLocalizedValue(StringId.KeyboardLayoutPrefix)} {keyboardName}";

            return layoutName;
        }

        internal CultureInfo GetLanguageCulture(IntPtr baseOsLayoutHandle)
        {
            try
            {
                // The Handle is a IntPtr with the 4 lower bytes for Language and the 4 higher bytes for Keyboard layout.
                // We take the language part.
                var cultureValue = (int)baseOsLayoutHandle & CultureMask;
                return new CultureInfo(cultureValue);
            }
            catch (CultureNotFoundException)
            {
                return null;
            }
        }

        private string GetLayoutId(IntPtr baseLayoutHandler)
        {
            var cultureValue = ((int)baseLayoutHandler >> 16) & CultureMask;

            // If the Layout ID begins by F, replace this character by 0 (ex: 0xF040 => 0x0040)
            if (cultureValue >= 0xF000)
            {
                cultureValue = cultureValue & 0x0FFF;
            }

            var layoutId = $"{cultureValue:x8}";

            return layoutId;
        }

        internal string GetKeyboardLayoutName(IntPtr baseLayoutHandler)
        {
            const string LayoutDisplayNameKey = "Layout Display Name";
            const string LayoutTextKey = "Layout Text";

            string displayName = null;
            var layoutId = GetLayoutId(baseLayoutHandler);

            var registryKeyName = "SYSTEM\\ControlSet001\\Control\\Keyboard Layouts";
            var registryKey = Registry.LocalMachine.OpenSubKey(registryKeyName);

            if (registryKey == null)
            {
                return null;
            }

            var keyName = registryKey.GetSubKeyNames().FirstOrDefault(x => x == layoutId);
            if (!string.IsNullOrEmpty(keyName))
            {
                var subKey = registryKey.OpenSubKey(keyName);
                if (subKey != null)
                {
                    var displayNameDirtyValue = subKey.GetValue(LayoutDisplayNameKey) as string;
                    displayName = IndirectStrings.GetIndirectString(displayNameDirtyValue);

                    var text = subKey.GetValue(LayoutTextKey) as string;
                }
            }

            return displayName;
        }

        private string GetKeyboardName(IntPtr baseOsLayoutHandle)
        {
            try
            {
                var cultureValue = ((int)baseOsLayoutHandle >> 16) & CultureMask;

                // If the Layout ID begins by F, replace this character by 0 (ex: 0xF040 => 0x0040)
                if (cultureValue >= 0xF000)
                {
                    cultureValue = cultureValue & 0x0FFF;
                }

                var layoutId = $"{cultureValue:x8}";

                // Retrieve the layout name in the static list.
                return _layoutNames[layoutId.ToUpper()];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        /// In window registry there are 2 layout key type:
        ///   - The standard type is coded 0000xxxx where xxxx is the layout ID.
        ///   - The special type is coded 0yyyyyyy. In this case the layout ID is a value of the key in the registry.
        /// So we create a dictionary with LayoutID/LayoutName for Keys/Values pairs.
        /// </summary>
        private void ConstructLayoutNameList()
        {
            var layoutKeys = Registry.LocalMachine.OpenSubKey($"SYSTEM\\CurrentControlSet\\Control\\Keyboard Layouts");
            if (layoutKeys == null)
            {
                return;
            }

            var layoutIds = layoutKeys.GetSubKeyNames();

            foreach (var layoutId in layoutIds)
            {
                AddLayoutName(layoutId);
            }
        }

        private void AddLayoutName(string layoutId)
        {
            const string StandardType = "0000";
            const string LayoutIdKey = "Layout Id";
            const string LayoutTextKey = "Layout Text";

            var registryKeyName = $"SYSTEM\\CurrentControlSet\\Control\\Keyboard Layouts\\{layoutId}";
            var registryKey = Registry.LocalMachine.OpenSubKey(registryKeyName);

            if (registryKey == null)
            {
                return;
            }

            var realLayoutId = layoutId;

            // If registry key don't start by 0000 it's not a standard type so use the "Layout Id" value instead of key name.
            if (!layoutId.StartsWith(StandardType))
            {
                realLayoutId = (string)registryKey.GetValue(LayoutIdKey);
            }

            if (realLayoutId == null)
            {
                return;
            }

            var layoutName = (string)registryKey.GetValue(LayoutTextKey);

            if (layoutName == null)
            {
                return;
            }

            _layoutNames.Add(realLayoutId.PadLeft(8, '0').ToUpper(), layoutName);
        }
    }
}
