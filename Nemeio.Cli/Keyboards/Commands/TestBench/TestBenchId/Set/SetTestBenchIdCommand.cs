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

namespace Nemeio.Cli.Keyboards.Commands.TestBench.TestBenchId.Set
{
    internal sealed class SetTestBenchIdCommand : KeyboardCommand
    {
        private readonly SetTestBenchIdInput _setTestBenchIdInput;

        public SetTestBenchIdCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source, SetTestBenchIdInput setTestBenchIdInput)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source)
        {
            if (setTestBenchIdInput == null)
            {
                throw new ArgumentNullException(nameof(setTestBenchIdInput));
            }

            _setTestBenchIdInput = setTestBenchIdInput;
        }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            var timeout = TimeSpan.FromSeconds(1);

            await Task.Run(() =>
            {
                try
                {
                    var setTestBenchIdMonitor = _monitorFactory.CreateSetTestBenchIdMonitor(nemeio.CommandExecutor);
                    var result = setTestBenchIdMonitor.SetTestBenchId(_setTestBenchIdInput.TestId);
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
                    var errorMessage = $"SetTestBenchId  failed";
                    _logger.LogError(errorMessage, exception);
                    throw new SetTestBenchIdFailedException("SetTestBenchId failed", exception);
                }
            });

        }
    }
}
