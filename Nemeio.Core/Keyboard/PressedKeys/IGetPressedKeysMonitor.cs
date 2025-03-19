using Nemeio.Core.Services.TestBench.PressedKeys;

namespace Nemeio.Core.Keyboard.PressedKeys
{
    public interface IGetPressedKeysMonitor
    {
        PressedKeysState GetPressedKeys();
    }
}
