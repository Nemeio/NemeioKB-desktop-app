using Nemeio.Core.DataModels;

namespace Nemeio.Core.Keyboard.Version
{
    public interface IVersionMonitor
    {
        FirmwareVersions AskVersions();
    }
}
