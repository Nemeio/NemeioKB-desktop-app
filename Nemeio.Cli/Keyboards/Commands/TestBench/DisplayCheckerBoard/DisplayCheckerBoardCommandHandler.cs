using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Handlers.Keyboards;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.DisplayCheckerBoard
{
    internal sealed class DisplayCheckerBoardCommandHandler : KeyboardCommandHandler, IDisplayCheckerBoardCommandHandler
    {
        private CommandOption _firstColor;
        public DisplayCheckerBoardCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("displayCheckerBoard", "Allows to display a checker board on the keyboard's screen")) { }

        public override void RegisterOptions(CommandLineApplication application)
        {
            _firstColor = application.Option("-f |--first <firstColor>", "Color of the first case (0 = black / 1 = white)", CommandOptionType.SingleValue);
        }

        public override async Task ExecuteAsync()
        {
            var firstColor = _firstColor;
            DisplayCheckerBoardInput displayCheckerBoardInput = new DisplayCheckerBoardInput(byte.Parse(firstColor.Value()));
            var cancellationTokenSource = new CancellationTokenSource();
            var command = new DisplayCheckerBoardCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource, displayCheckerBoardInput);

            await command.ExecuteAsync();
        }
    }
}
