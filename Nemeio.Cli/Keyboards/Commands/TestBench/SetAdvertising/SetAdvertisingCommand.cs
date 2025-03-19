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

namespace Nemeio.Cli.Keyboards.Commands.TestBench.SetAdvertising
{
    internal sealed class SetAdvertisingCommand : KeyboardCommand
    {
        private readonly SetAdvertisingInput _setAdvertisingInput;

        public SetAdvertisingCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source, SetAdvertisingInput setAdvertisingInput)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source)
        {
            if (setAdvertisingInput == null)
            {
                throw new ArgumentNullException(nameof(setAdvertisingInput));
            }

            _setAdvertisingInput = setAdvertisingInput;
        }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            var timeout = TimeSpan.FromSeconds(1);

            await Task.Run(() =>
            {
                try
                {
                    var setAdvertisingMonitor = _monitorFactory.CreateSetAdvertisingMonitor(nemeio.CommandExecutor);
                    var result = setAdvertisingMonitor.SetAdvertising((byte)_setAdvertisingInput.Enable);
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
                    var errorMessage = $"SetAdvertising  failed";
                    _logger.LogError(errorMessage, exception);
                    _outputWriter.WriteErrorLine(errorMessage);
                    throw new SetAdvertisingFailedException("SetAdvertising failed", exception);
                }
            });

        }
    }
}
