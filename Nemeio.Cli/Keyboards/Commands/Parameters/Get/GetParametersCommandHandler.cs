using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Handlers.Keyboards;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.Parameters.Get
{
    internal sealed class GetParametersCommandHandler : KeyboardCommandHandler, IGetParametersCommandHandler
    {
        public GetParametersCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory) 
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("getParameters", "Get keyboard's parameters")) { }

        public override void RegisterOptions(CommandLineApplication application)
        {
            //  Nothing to do here
            //  This command not support options
        }

        public override async Task ExecuteAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var command = new GetParametersCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource);

            await command.ExecuteAsync();
        }
    }
}
