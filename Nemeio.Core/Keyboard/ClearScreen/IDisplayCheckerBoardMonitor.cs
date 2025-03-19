using Nemeio.Core.Services.Batteries;

namespace Nemeio.Core.Keyboard.DisplayCheckerBoard
{
    public interface IDisplayCheckerBoardMonitor
    {
        GenericTestBenchResult DisplayCheckerBoard(byte firstColor);
    }
}
