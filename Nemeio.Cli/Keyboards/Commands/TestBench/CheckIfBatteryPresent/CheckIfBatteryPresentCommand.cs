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
using Nemeio.Core.Services.TestBench;
using Newtonsoft.Json;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.CheckIfBatteryPresent
{
    internal sealed class CheckIfBatteryPresentCommand : KeyboardCommand
    {


        public CheckIfBatteryPresentCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source)
        {
        }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            var timeout = TimeSpan.FromSeconds(1);

            await Task.Run(() =>
            {
                try
                {
                    var checkIfBatteryPresentMonitor = _monitorFactory.CreateCheckIfBatteryPresentMonitor(nemeio.CommandExecutor);
                    var output = checkIfBatteryPresentMonitor.CheckIfBatteryPresent();
                    _outputWriter.WriteObject(output);
                }
                catch (KeyboardException keyboardException)
                {
                    var output = new CheckIfBatteryPresentResult(keyboardException.ErrorCode, false);
                    _logger.LogError(this.GetType().Name, keyboardException);
                    _outputWriter.WriteObject(output);
                }
                catch (Exception exception)
                {
                    var errorMessage = $"CheckIfBatteryPresent  failed";
                    _logger.LogError(errorMessage, exception);
                    throw new CheckIfBatteryPresentFailedException("CheckIfBatteryPresent failed", exception);
                }
            });

        }
    }
}
