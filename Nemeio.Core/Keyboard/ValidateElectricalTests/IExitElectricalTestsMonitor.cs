using Nemeio.Core.Services.Batteries;

namespace Nemeio.Core.Keyboard.ExitElectricalTests
{
    public interface IExitElectricalTestsMonitor
    {
        GenericTestBenchResult ExitElectricalTest(byte validationState);
    }
}
