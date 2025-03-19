using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Handlers.Keyboards;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.Version
{
    internal sealed class VersionCommandHandler : KeyboardCommandHandler, IVersionCommandHandler
    {
        public VersionCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory) 
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("version", "Get keyboard's version")) { }

        public override void RegisterOptions(CommandLineApplication application)
        {
            //  Nothing to do here
        }

        public override async Task ExecuteAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var command = new VersionCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource);

            await command.ExecuteAsync();
        }
    }
}
