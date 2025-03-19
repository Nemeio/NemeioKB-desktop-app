using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Handlers.Keyboards;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.ClearScreen
{
    internal sealed class ClearScreenCommandHandler : KeyboardCommandHandler, IClearScreenCommandHandler
    {
        public ClearScreenCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("clearScreen", "Allows to clear the keyboard's screen")) { }

        public override void RegisterOptions(CommandLineApplication application)
        {
            //Nothing to do here
        }

        public override async Task ExecuteAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var command = new ClearScreenCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource);

            await command.ExecuteAsync();
        }
    }
}
