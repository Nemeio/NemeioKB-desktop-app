using Nemeio.Core.Services.Batteries;

namespace Nemeio.Core.Keyboard.ExitFunctionalTests
{
    public interface IExitFunctionalTestsMonitor
    {
        GenericTestBenchResult ValidateFunctionalTest(byte validationState);
    }
}
