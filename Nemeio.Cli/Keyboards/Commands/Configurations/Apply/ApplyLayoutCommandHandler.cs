using System.Threading;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.Configurations.Apply
{
    internal sealed class ApplyLayoutCommandHandler : LayoutHashCommandHandler, IApplyLayoutCommandHandler
    {
        private CommandOption _waitChangeLayout;

        public ApplyLayoutCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory) 
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("apply", "Apply layout on keyboard")) { }

        public override string GetHashOptionDescription() => "Layout's hash to apply on keyboard";

        public override void RegisterOptions(CommandLineApplication application)
        {
            base.RegisterOptions(application);

            _waitChangeLayout = application.Option("-switch |--switch <switch>", "Must wait switch layout", CommandOptionType.NoValue);
        }

        public override LayoutHashCommand CreateCommand(CancellationTokenSource cancellationTokenSource, string hash)
        {
            var needWait = _waitChangeLayout.HasValue();
            var command = new ApplyLayoutCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource, hash, needWait);

            return command;
        }
    }
}
