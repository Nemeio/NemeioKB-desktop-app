using Nemeio.Core.Services.Batteries;

namespace Nemeio.Core.Keyboard.SetLed
{
    public interface ISetLedMonitor
    {
        GenericTestBenchResult SetLed(byte ledId, byte ledState);
    }
}
