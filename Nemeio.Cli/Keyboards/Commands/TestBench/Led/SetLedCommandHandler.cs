using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Handlers.Keyboards;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.Led
{
    internal sealed class SetLedCommandHandler : KeyboardCommandHandler, ISetLedCommandHandler
    {
        private CommandOption _ledId;
        private CommandOption _ledState;

        public SetLedCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("setLed", "Allow you to set Led state")) { }

        public override void RegisterOptions(CommandLineApplication application)
        {
            _ledId = application.Option("-l |--led <led>", "id of the Led ()", CommandOptionType.SingleValue);
            _ledState = application.Option("-s |--state <state>", "desired state (0=off / 1=on)", CommandOptionType.SingleValue);
        }

        public override async Task ExecuteAsync()
        {
            var id = _ledId;
            var state = _ledState;
            LedInput ledInput = new LedInput(uint.Parse(id.Value()), uint.Parse(state.Value()));

            var cancellationTokenSource = new CancellationTokenSource();
            var command = new SetLedCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource, ledInput);

            await command.ExecuteAsync();
        }
    }
}
