using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Handlers.Keyboards;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.FunctionalTests
{
    internal sealed class ExitFunctionalTestsCommandHandler : KeyboardCommandHandler, IExitFunctionalTestsCommandHandler
    {
        private CommandOption _validationState;
        public ExitFunctionalTestsCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("exitFunctionalTests", "Allow to exit the functional tests")) { }

        public override void RegisterOptions(CommandLineApplication application)
        {
            _validationState = application.Option("-s |--state <state>", "Desired state (0 = Not validated / 1 = validated / 2 = ignore)", CommandOptionType.SingleValue);
        }

        public override async Task ExecuteAsync()
        {
            byte targetState = byte.Parse(_validationState.Value());
            var cancellationTokenSource = new CancellationTokenSource();
            var command = new ExitFunctionalTestsCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource, targetState);

            await command.ExecuteAsync();
        }
    }
}
