using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts.Images.AugmentedLayout
{
    public interface IAugmentedLayoutImageProvider
    {
        bool AugmentedLayoutImageExists(OsLayoutId layoutId, LayoutImageType imageType);

        bool AugmentedLayoutImageExists(ILayout layout);

        byte[] GetAugmentedLayoutImage(OsLayoutId layoutId, LayoutImageType imageType);
    }
}
