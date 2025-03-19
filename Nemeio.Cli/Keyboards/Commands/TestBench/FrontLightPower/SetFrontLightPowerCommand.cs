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

namespace Nemeio.Cli.Keyboards.Commands.TestBench.FrontLightPower
{
    internal sealed class SetFrontLightPowerCommand : KeyboardCommand
    {
        private readonly FrontLightPowerInput _frontLightPowerInput;

        public SetFrontLightPowerCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source, FrontLightPowerInput frontLightPowerInput)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source)
        {
            if (frontLightPowerInput == null)
            {
                throw new ArgumentNullException(nameof(frontLightPowerInput));
            }

            _frontLightPowerInput = frontLightPowerInput;
        }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            var timeout = TimeSpan.FromSeconds(1);

            await Task.Run(() =>
            {
                try
                {
                    var frontLightPowerMonitor = _monitorFactory.CreateSetFrontLightPowerMonitor(nemeio.CommandExecutor);
                    var result = frontLightPowerMonitor.SetFrontLightPower((byte)_frontLightPowerInput.Power);
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
                    var errorMessage = $"SetFrontLightPower  failed";
                    _logger.LogError(errorMessage, exception);
                    _outputWriter.WriteErrorLine(errorMessage);
                    throw new SetFrontLightPowerFailedException("SetFrontLightPower failed", exception);
                }
            });

        }
    }
}
