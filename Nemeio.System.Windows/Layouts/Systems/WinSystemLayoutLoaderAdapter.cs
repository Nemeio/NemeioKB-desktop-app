using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Win32;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using Nemeio.Windows.Win32;

namespace Nemeio.Platform.Windows.Layouts.Systems
{
    public class WinSystemLayoutLoaderAdapter : ISystemLayoutLoaderAdapter
    {
        private readonly WinOsLayoutIdBuilder _osLayoutIdBuilder;

        public WinSystemLayoutLoaderAdapter(WinOsLayoutIdBuilder osLayoutIdBuilder)
        {
            _osLayoutIdBuilder = osLayoutIdBuilder ?? throw new ArgumentNullException(nameof(osLayoutIdBuilder));
        }

        public IEnumerable<OsLayoutId> Load()
        {
            var count = WinUser32.GetKeyboardLayoutList(0, null);
            var idArray = new IntPtr[count];
            var nameArray = new IntPtr[count];
            WinUser32.GetKeyboardLayoutList(idArray.Length, idArray);
            WinUser32.GetKeyboardLayoutNameA(nameArray);
            Dictionary<IntPtr, CultureInfo> map = new Dictionary<IntPtr, CultureInfo>();
            var regKey = WinUser32.OpenSubKey("Control Panel\\International\\User Profile", true);

            var t = regKey.GetValueNames();
            RegistryKey langsKey = Registry.CurrentUser.OpenSubKey("Control Panel\\International\\User Profile", true);
            var langs = (string[])langsKey.GetValue("Languages", "");

            foreach (var id in idArray)
            {
                map.Add(id, _osLayoutIdBuilder.GetLanguageCulture(id));
            }

            List<IntPtr> result = new List<IntPtr>();

            List<CultureInfo> notfound = new List<CultureInfo>();
            foreach (var lang in langs)
            {
                var res = CultureInfo.GetCultureInfo(lang);
                var first = map.FirstOrDefault(x => x.Value.Equals(res)).Key;
                if (first != IntPtr.Zero)
                {
                    result.Add(first);
                }
               
            }
            
            result.AddRange(map.Select(x => x.Key).Except(result));
            int order = 0;

            var ordered = result
            .Take(count)
            .Select(i => _osLayoutIdBuilder.Build(i)).ToList();

            ordered.ForEach(x => x.Order = order++);

            return ordered;
        }
    }
}
