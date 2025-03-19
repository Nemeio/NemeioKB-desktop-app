using Nemeio.Core.Applications;
using Nemeio.Core.Layouts.Images.AugmentedLayout;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Resources;
using Nemeio.Core.Systems.Keyboards;
using Nemeio.Layout.Builder;
using Nemeio.Platform.Mac.Keyboards;

namespace Nemeio.Mac.Services
{
    public class MacLayoutImageGenerator : LayoutImageGenerator
    {
        public MacLayoutImageGenerator(IFontProvider fontProvider, IApplicationSettingsProvider appSettingsManager, IAugmentedLayoutImageProvider augmentedLayoutImageProvider, IResourceLoader resourceLoader) 
            : base(fontProvider, appSettingsManager, augmentedLayoutImageProvider, resourceLoader) { }

        public override ISystemKeyboardBuilder CreateKeyboardBuilder() => new MacKeyboardBuilder();
    }
}
