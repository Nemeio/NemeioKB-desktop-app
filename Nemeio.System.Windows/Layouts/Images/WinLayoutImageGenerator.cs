using System;
using System.IO;
using Nemeio.Core.Applications;
using Nemeio.Core.Layouts.Images.AugmentedLayout;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Resources;
using Nemeio.Core.Systems.Keyboards;
using Nemeio.Layout.Builder;
using Nemeio.Platform.Windows.Keyboards;

namespace Nemeio.Platform.Windows.Layouts.Images
{
    public sealed class WinLayoutImageGenerator : LayoutImageGenerator
    {
     	private readonly WinOsLayoutIdBuilder _osLayoutIdBuilder;
     
        public WinLayoutImageGenerator(IFontProvider fontProvider, IApplicationSettingsProvider applicationSettingsProvider, IAugmentedLayoutImageProvider augmentedLayoutImageProvider, WinOsLayoutIdBuilder osLayoutIdBuilder, IResourceLoader resourceLoader) 
            : base(fontProvider, applicationSettingsProvider, augmentedLayoutImageProvider, resourceLoader)
        {
            _osLayoutIdBuilder = osLayoutIdBuilder ?? throw new ArgumentNullException(nameof(osLayoutIdBuilder));
        }

        public override ISystemKeyboardBuilder CreateKeyboardBuilder() => new WinKeyboardBuilder(_resourceLoader, _osLayoutIdBuilder);
    }
}
