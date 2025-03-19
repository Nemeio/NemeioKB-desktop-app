using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Commands
{
    internal abstract class KeyboardCommand : Command
    {
        private readonly IKeyboardAwaiterFactory _keyboardAwaiterFactory;
        protected readonly IMonitorFactory _monitorFactory;
        protected readonly ILogger _logger;

        protected KeyboardCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source)
            : base(loggerFactory, outputWriter, source)
        {
            _logger = loggerFactory.CreateLogger<KeyboardCommand>();
            _keyboardAwaiterFactory = keyboardAwaiterFactory ?? throw new ArgumentNullException(nameof(keyboardAwaiterFactory));
            _monitorFactory = monitorFactory ?? throw new ArgumentNullException(nameof(monitorFactory));
        }

        public override async Task ExecuteAsync()
        {
            using (var keyboardAwaiter = _keyboardAwaiterFactory.Create())
            {
                _logger.LogInformation($"Waiting keyboard ...");

                var nemeio = await keyboardAwaiter.WaitKeyboardAsync(CancellationTokenSource);
                nemeio.OnDisconnected += Nemeio_OnDisconnected;
                
                await nemeio.CommandExecutor.Initialize();

                _logger.LogInformation($"Keyboard found <{nemeio.Keyboard.Identifier}>");

                await ApplyAsync(nemeio);

                nemeio.OnDisconnected -= Nemeio_OnDisconnected;
                await nemeio.CommandExecutor.StopAsync();
            }
        }

        private void Nemeio_OnDisconnected(object sender, EventArgs e) => OnKeyboardDisconnected();

        public abstract Task ApplyAsync(Keyboards.Nemeio nemeio);

        protected virtual void OnKeyboardDisconnected()
        {
            //  Nothing to do here
            //  Can be subclassed
        }
    }
}
