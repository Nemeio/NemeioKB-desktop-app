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

namespace Nemeio.Cli.Keyboards.Commands.TestBench.BatteryElectricalStatus
{
    internal sealed class GetBatteryElectricalStatusCommand : KeyboardCommand
    {
        public GetBatteryElectricalStatusCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source, byte validationState)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source) { }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            var timeout = TimeSpan.FromSeconds(1);

            await Task.Run(() =>
            {
                BatteryElectricalState result = null;
                try
                {
                    var batteryElectricalStatusMonitor = _monitorFactory.CreateGetBatteryStatusMonitor(nemeio.CommandExecutor);
                    result = batteryElectricalStatusMonitor.GetBatteryElectricalStatus();
                    _outputWriter.WriteObject(result);
                }
                catch (KeyboardException keyboardException)
                {
                    var output = new BatteryElectricalState(keyboardException.ErrorCode, 0, 0);
                    _logger.LogError(this.GetType().Name, keyboardException);
                    _outputWriter.WriteObject(output);
                }
                catch (Exception exception)
                {
                    var errorMessage = $"ExitElectricalTests failed";
                    _logger.LogError(errorMessage, exception);
                    _outputWriter.WriteObject(exception.Message);
                    throw new GetBatteryElectricalStatusFailedException("GetBatteryElectricalStatus failed", exception);
                }
            });
        }
    }
}
