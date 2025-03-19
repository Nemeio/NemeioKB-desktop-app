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

namespace Nemeio.Cli.Keyboards.Commands.TestBench.CheckComponents
{
    internal sealed class CheckComponentsCommand : KeyboardCommand
    {
        private readonly CheckComponentsInput _checkComponentsInput;

        public CheckComponentsCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source, CheckComponentsInput checkComponentsInput)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source)
        {
            if (checkComponentsInput == null)
            {
                throw new ArgumentNullException(nameof(checkComponentsInput));
            }

            _checkComponentsInput = checkComponentsInput;
        }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            var timeout = TimeSpan.FromSeconds(1);

            await Task.Run(() =>
            {
                CheckComponentResult result = null;
                try
                {
                    var checkComponentsMonitor = _monitorFactory.CreateCheckComponentsMonitor(nemeio.CommandExecutor);
                    result = checkComponentsMonitor.CheckComponent((byte)_checkComponentsInput.ComponentId);
                    _outputWriter.WriteObject(result);
                }

                catch (KeyboardException keyboardException)
                {
                    result = new CheckComponentResult(keyboardException.ErrorCode);
                    _logger.LogError(this.GetType().Name, keyboardException);
                    _outputWriter.WriteObject(result);
                }
                catch (Exception exception)
                {
                    var errorMessage = $"CheckComponents failed";
                    _logger.LogError(errorMessage, exception);
                    throw new CheckComponentsFailedException("CheckComponents failed", exception);
                }
            });
        }
    }
}
