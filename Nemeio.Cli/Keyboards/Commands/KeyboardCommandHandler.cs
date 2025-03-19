using System;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Cli.Keyboards.Commands;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Commands.Handlers.Keyboards
{
    internal abstract class KeyboardCommandHandler : CommandHandler
    {
        protected readonly IKeyboardAwaiterFactory _keyboardAwaiterFactory;
        protected readonly IMonitorFactory _monitorFactory;

        protected KeyboardCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter,  IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CommandInfo info)
            : base(loggerFactory, outputWriter, info)
        {
            _keyboardAwaiterFactory = keyboardAwaiterFactory ?? throw new ArgumentNullException(nameof(keyboardAwaiterFactory));
            _monitorFactory = monitorFactory ?? throw new ArgumentNullException(nameof(monitorFactory));
        }
    }
}
