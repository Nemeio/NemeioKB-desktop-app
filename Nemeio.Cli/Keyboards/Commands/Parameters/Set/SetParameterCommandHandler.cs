using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Handlers.Keyboards;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.Parameters.Set
{
    internal sealed class SetParameterCommandHandler : KeyboardCommandHandler, ISetParameterCommandHandler
    {
        private CommandOption _jsonParameters;

        public SetParameterCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory) 
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("setParameters", "Allow you to set parameters")) { }

        public override void RegisterOptions(CommandLineApplication application)
        {
            _jsonParameters = application.Option("-json |--json <json>", "Parameters you want to override with JSON format", CommandOptionType.SingleValue);
        }

        public override async Task ExecuteAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var command = new SetParameterCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource, _jsonParameters.Value());

            await command.ExecuteAsync();
        }
    }
}
