using Nemeio.Core.Services.Batteries;

namespace Nemeio.Core.Keyboard.SetAdvertising
{
    public interface ISetAdvertisingMonitor
    {
        GenericTestBenchResult SetAdvertising(byte enable);
    }
}
