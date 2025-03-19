using System;
using Nemeio.Core.Layouts.Name;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Platform.Windows.Layouts
{
    public class WinLayoutNativeNameAdapter : ILayoutNativeNameAdapter
    {
        private readonly WinOsLayoutIdBuilder _osLayoutIdBuilder;

        public WinLayoutNativeNameAdapter(WinOsLayoutIdBuilder osLayoutIdBuilder)
        {
            _osLayoutIdBuilder = osLayoutIdBuilder ?? throw new ArgumentNullException(nameof(osLayoutIdBuilder));
        }

        public string RetrieveNativeName(OsLayoutId osLayoutId)
        {
            var windowSystemLayout = _osLayoutIdBuilder.Parse(osLayoutId);

            return windowSystemLayout.Name;
        }
    }
}
