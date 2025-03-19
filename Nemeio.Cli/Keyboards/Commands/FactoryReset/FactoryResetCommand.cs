using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.FactoryReset;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.FactoryReset
{
    internal sealed class FactoryResetCommand : KeyboardCommand
    {
        private readonly SemaphoreSlim _waitKeyboardDisconnectEvent;

        public FactoryResetCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source)
        {
            _waitKeyboardDisconnectEvent = new SemaphoreSlim(0, 1);
        }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            var timeout = TimeSpan.FromSeconds(5);
            var factoryResetMonitor = _monitorFactory.CreateFactoryResetMonitor(nemeio.CommandExecutor);

            Reset(factoryResetMonitor);

            if (!nemeio.IsDisconnected)
            {
                var succeed = await _waitKeyboardDisconnectEvent.WaitAsync(timeout);
                if (!succeed)
                {
                    throw new FactoryResetFailedException($"Keyboard never disconnect");
                }
            }
        }

        private void Reset(IFactoryResetMonitor monitor)
        {
            //  Keyboard never respond
            //  We not wait

            Task.Factory.StartNew(() => 
            {
                try
                {
                    monitor.AskFactoryReset();
                }
                catch (Exception exception)
                {
                    var errorMessage = $"Factory reset failed";

                    _logger.LogError(exception, errorMessage);

                    throw new FactoryResetFailedException(errorMessage, exception);
                }
            });
        }

        protected override void OnKeyboardDisconnected()
        {
            base.OnKeyboardDisconnected();

            _waitKeyboardDisconnectEvent.Release();
        }
    }
}
