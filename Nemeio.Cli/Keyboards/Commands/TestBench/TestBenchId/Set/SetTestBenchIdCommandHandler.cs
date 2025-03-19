using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Handlers.Keyboards;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.TestBenchId.Set
{
    internal sealed class SetTestBenchIdCommandHandler : KeyboardCommandHandler, ISetTestBenchIdCommandHandler
    {
        private CommandOption _power;


        public SetTestBenchIdCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("setTestBenchId", "Allow to define a TestBench Id")) { }

        public override void RegisterOptions(CommandLineApplication application)
        {
            _power = application.Option("-i |--id <id>", "34 bytes representing the Id", CommandOptionType.SingleValue);
        }

        public override async Task ExecuteAsync()
        {
            var id = _power;
            SetTestBenchIdInput setTestBenchIdInput = new SetTestBenchIdInput(id.Value());

            var cancellationTokenSource = new CancellationTokenSource();
            var command = new SetTestBenchIdCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource, setTestBenchIdInput);

            await command.ExecuteAsync();
        }
    }
}
