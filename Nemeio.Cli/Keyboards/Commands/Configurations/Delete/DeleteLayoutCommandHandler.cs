using System.Threading;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.Configurations.Delete
{
    internal sealed class DeleteLayoutCommandHandler : LayoutHashCommandHandler, IDeleteLayoutCommandHandler
    {
        public DeleteLayoutCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory) 
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("delete", "Remove layout on keyboard")) { }

        public override string GetHashOptionDescription() => "Layout's hash to delete";

        public override LayoutHashCommand CreateCommand(CancellationTokenSource cancellationTokenSource, string hash)
        {
            var command = new DeleteLayoutCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource, hash);

            return command;
        }
    }
}
