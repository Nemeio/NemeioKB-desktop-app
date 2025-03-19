using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Handlers.Keyboards;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.CheckComponents
{
    internal sealed class CheckComponentsCommandHandler : KeyboardCommandHandler, ICheckComponentsCommandHandler
    {
        private CommandOption _componentId;

        public CheckComponentsCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("checkComponents", "Allow to test component")) { }

        public override void RegisterOptions(CommandLineApplication application)
        {
            _componentId = application.Option("-c |--component <component>", "id of the Component ()", CommandOptionType.SingleValue);
        }

        public override async Task ExecuteAsync()
        {
            var id = _componentId;
            CheckComponentsInput checkComponentInput = new CheckComponentsInput(uint.Parse(id.Value()));

            var cancellationTokenSource = new CancellationTokenSource();
            var command = new CheckComponentsCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource, checkComponentInput);

            await command.ExecuteAsync();
        }
    }
}
