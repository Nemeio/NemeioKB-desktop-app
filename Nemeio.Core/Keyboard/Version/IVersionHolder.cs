using Nemeio.Core.DataModels;

namespace Nemeio.Core.Keyboard.Version
{
    public interface IVersionHolder
    {
        FirmwareVersions Versions { get; }
    }
}
