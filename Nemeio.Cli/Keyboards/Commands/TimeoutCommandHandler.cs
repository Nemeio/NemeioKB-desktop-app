using System.Threading;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Exceptions;
using Nemeio.Cli.Commands.Handlers.Keyboards;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands
{
    internal abstract class TimeoutCommandHandler : KeyboardCommandHandler
    {
        private CommandOption _timeoutOption;

        public TimeoutCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CommandInfo commandInfo)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, commandInfo) { }

        public override void RegisterOptions(CommandLineApplication application)
        {
            _timeoutOption = application.Option("-timeout |--timeout <timeout>", "Timeout to stop waiting", CommandOptionType.SingleValue);
        }

        public CancellationTokenSource BuildCancellationToken()
        {
            CancellationTokenSource cancellationTokenSource;

            if (_timeoutOption.HasValue())
            {
                var strTimeout = _timeoutOption.Value();
                var succeed = int.TryParse(strTimeout, out var timeout);
                if (!succeed)
                {
                    throw new InvalidParameterException($"Timeout must be a valid integer, but was <{strTimeout}>");
                }

                cancellationTokenSource = new CancellationTokenSource(timeout);
            }
            else
            {
                cancellationTokenSource = new CancellationTokenSource();
            }

            return cancellationTokenSource;
        }
    }
}
