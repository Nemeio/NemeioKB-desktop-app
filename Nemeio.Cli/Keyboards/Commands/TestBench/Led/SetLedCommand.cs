using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Exceptions;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Services.Batteries;
using Newtonsoft.Json;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.Led
{
    internal sealed class SetLedCommand : KeyboardCommand
    {
        private readonly LedInput _ledInput;

        public SetLedCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source, LedInput ledInput)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source)
        {
            if (ledInput == null)
            {
                throw new ArgumentNullException(nameof(ledInput));
            }

            _ledInput = ledInput;
        }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            var timeout = TimeSpan.FromSeconds(1);

            await Task.Run(() =>
            {
                try
                {
                    var ledMonitor = _monitorFactory.CreateSetLedMonitor(nemeio.CommandExecutor);
                    var result = ledMonitor.SetLed((byte)_ledInput.LedId, (byte)_ledInput.LedState);
                    _outputWriter.WriteObject(result);
                }
                catch (KeyboardException keyboardException)
                {
                    var output = new GenericTestBenchResult(keyboardException.ErrorCode);
                    _logger.LogError(this.GetType().Name, keyboardException);
                    _outputWriter.WriteObject(output);
                }
                catch (Exception exception)
                {
                    var errorMessage = $"SetLed failed";
                    _logger.LogError(errorMessage, exception);
                    throw new SetLedFailedException("SetLed failed", exception);
                }
            });

        }
    }
}
