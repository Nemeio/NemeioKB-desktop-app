using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.Configurations
{
    internal abstract class LayoutHashCommandHandler : TimeoutCommandHandler
    {
        private CommandOption _layoutHash;

        public LayoutHashCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CommandInfo info)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, info) { }

        public abstract string GetHashOptionDescription();

        public abstract LayoutHashCommand CreateCommand(CancellationTokenSource cancellationTokenSource, string hash);

        public override void RegisterOptions(CommandLineApplication application)
        {
            base.RegisterOptions(application);

            _layoutHash = application.Option("-hash |--hash <hash>", GetHashOptionDescription(), CommandOptionType.SingleValue);
        }

        public override async Task ExecuteAsync()
        {
            var cancellationTokenSource = BuildCancellationToken();
            var command = CreateCommand(cancellationTokenSource, _layoutHash.Value());

            await command.ExecuteAsync();
        }
    }
}
