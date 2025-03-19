using Nemeio.Core.Services.Batteries;

namespace Nemeio.Core.Keyboard.SetFrontLightPower
{
    public interface ISetFrontLightPowerMonitor
    {
        GenericTestBenchResult SetFrontLightPower(byte power);
    }
}
