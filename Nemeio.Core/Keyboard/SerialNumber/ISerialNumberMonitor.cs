using Nemeio.Core.DataModels;

namespace Nemeio.Core.Keyboard.SerialNumber
{
    public interface ISerialNumberMonitor
    {
        NemeioSerialNumber AskSerialNumber();
    }
}
