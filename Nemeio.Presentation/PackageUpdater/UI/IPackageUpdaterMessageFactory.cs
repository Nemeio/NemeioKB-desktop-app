using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Keyboard.Updates.Progress;
using Nemeio.Core.PackageUpdater;

namespace Nemeio.Presentation.PackageUpdater.UI
{
    public interface IPackageUpdaterMessageFactory
    {
        StringId GetStatusMessageForCurrentState();
        PackageUpdaterMessage GetMessage(PackageUpdateState step);
        StringId GetNameForComponent(UpdateComponent component);
    }
}
