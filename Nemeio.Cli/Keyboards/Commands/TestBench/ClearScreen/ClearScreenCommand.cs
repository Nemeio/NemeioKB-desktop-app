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

namespace Nemeio.Cli.Keyboards.Commands.TestBench.ClearScreen
{
    internal sealed class ClearScreenCommand : KeyboardCommand
    {
        public ClearScreenCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source) { }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            var timeout = TimeSpan.FromSeconds(5);

            await Task.Run(() =>
            {
                GenericTestBenchResult result = null;
                try
                {
                    var clearScreenMonitor = _monitorFactory.CreateClearScreenMonitor(nemeio.CommandExecutor);
                     result = clearScreenMonitor.ClearScreen();
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
                    var errorMessage = $"ClearScreen failed";
                    _logger.LogError(errorMessage, exception);
                    _outputWriter.WriteObject(exception.Message);
                    throw new ClearScreenFailedException("ClearScreen failed", exception);
                }
            });
        }
    }
}
