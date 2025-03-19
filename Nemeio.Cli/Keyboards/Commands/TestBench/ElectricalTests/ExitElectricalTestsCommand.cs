using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Exceptions;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;
using Newtonsoft.Json;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.ElectricalTests
{
    internal sealed class ExitElectricalTestsCommand : KeyboardCommand
    {
        private readonly byte _validationState;
        public ExitElectricalTestsCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source, byte validationState)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source) {
            _validationState = validationState;
        }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            var timeout = TimeSpan.FromSeconds(5);

            await Task.Run(() =>
            {
                try
                {
                    var electricalTestsMonitor = _monitorFactory.CreateExitElectricalTestsMonitor(nemeio.CommandExecutor);
                    electricalTestsMonitor.ExitElectricalTest(_validationState);
                    _outputWriter.WriteObject(new Core.Services.Batteries.GenericTestBenchResult(Core.Keyboard.Communication.Errors.KeyboardErrorCode.Success));
                }
                catch (Exception exception)
                {
                    var errorMessage = $"ExitElectricalTests failed";
                    _logger.LogError(errorMessage, exception);
                    
                    throw new ExitElectricalTestsFailedException("ExitElectricalTests failed", exception);
                }
            });

        }
    }
}
