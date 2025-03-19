using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Exceptions;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Cli.Keyboards.Commands.Configurations
{
    internal abstract class LayoutHashCommand : KeyboardCommand
    {
        private readonly string _layoutId;

        public LayoutHashCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source, string layoutId)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source)
        {
            if (string.IsNullOrEmpty(layoutId))
            {
                throw new ArgumentNullException(nameof(layoutId));
            }

            _layoutId = layoutId;
        }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            LayoutId id = null;

            try
            {
                id = new LayoutId(_layoutId);
            }
            catch (ArgumentException exception)
            {
                _logger.LogError(exception, $"Parse layout hash failed");

                throw new InvalidParameterException();
            }

            await RunAsync(nemeio, id);
        }

        public abstract Task RunAsync(Nemeio nemeio, LayoutId id);
    }
}
