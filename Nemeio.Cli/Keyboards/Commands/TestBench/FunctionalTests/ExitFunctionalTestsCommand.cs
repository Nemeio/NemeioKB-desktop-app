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

namespace Nemeio.Cli.Keyboards.Commands.TestBench.FunctionalTests
{
    internal sealed class ExitFunctionalTestsCommand : KeyboardCommand
    {
        private readonly byte _validationState;
        public ExitFunctionalTestsCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source, byte validationState)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source)
        {
            _validationState = validationState;
        }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            var timeout = TimeSpan.FromSeconds(5);

            await Task.Run(() =>
            {
                try
                {
                    var FunctionalTestsMonitor = _monitorFactory.CreateExitFunctionalTestsMonitor(nemeio.CommandExecutor);
                    var result = FunctionalTestsMonitor.ValidateFunctionalTest(_validationState);
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
                    var errorMessage = $"ExitFunctionalTests failed";
                    _outputWriter.WriteLine(exception.Message);
                    _logger.LogError(errorMessage, exception);
                    throw new ExitFunctionalTestsFailedException("ExitFunctionalTests failed", exception);
                }
            });

        }
    }
}
