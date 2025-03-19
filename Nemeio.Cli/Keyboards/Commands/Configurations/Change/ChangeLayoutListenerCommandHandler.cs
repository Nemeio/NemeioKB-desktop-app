using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.Configurations.Change
{
    internal sealed class ChangeLayoutListenerCommandHandler : TimeoutCommandHandler, IChangeLayoutListenerCommandHandler
    {
        public ChangeLayoutListenerCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory) 
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("listen", "Wait keyboard layout change event")) { }

        public override async Task ExecuteAsync()
        {
            var cancellationTokenSource = BuildCancellationToken();
            var command = new ChangeLayoutListenerCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource);

            await command.ExecuteAsync();
        }
    }
}
