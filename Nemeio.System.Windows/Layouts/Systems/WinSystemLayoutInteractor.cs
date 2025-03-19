using System;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using Nemeio.Windows.Win32;

namespace Nemeio.Platform.Windows.Layouts.Systems
{
    public class WinSystemLayoutInteractor : ISystemLayoutInteractor
    {
        private readonly WinOsLayoutIdBuilder _osLayoutIdBuilder;

        public WinSystemLayoutInteractor(WinOsLayoutIdBuilder osLayoutIdBuilder)
        {
            _osLayoutIdBuilder = osLayoutIdBuilder ?? throw new ArgumentNullException(nameof(osLayoutIdBuilder));
        }

        public void ChangeSelectedLayout(OsLayoutId layoutid)
        {
            var targetedLayout = _osLayoutIdBuilder.Parse(layoutid);
            
            WinUser32.RequestLanguageChange(targetedLayout);
        }
    }
}
