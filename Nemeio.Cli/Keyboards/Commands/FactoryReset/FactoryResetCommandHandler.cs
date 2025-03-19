using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Handlers.Keyboards;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.FactoryReset
{
    internal sealed class FactoryResetCommandHandler : KeyboardCommandHandler, IFactoryResetCommandHandler
    {
        public FactoryResetCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory) 
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("factoryReset", "Reset keyboard to factory settings")) { }

        public override void RegisterOptions(CommandLineApplication application)
        {
            //  Nothing to do here
            //  No custom options for factory reset
        }

        public override async Task ExecuteAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var command = new FactoryResetCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource);

            await command.ExecuteAsync();
        }
    }
}
