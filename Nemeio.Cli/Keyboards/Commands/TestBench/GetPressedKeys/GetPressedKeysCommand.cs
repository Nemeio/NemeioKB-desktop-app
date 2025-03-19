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
using Nemeio.Core.Services.TestBench.PressedKeys;
using Newtonsoft.Json;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.PressedKeys
{
    internal sealed class GetPressedKeysCommand : KeyboardCommand
    {
        public GetPressedKeysCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source, byte validationState)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source) { }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            var timeout = TimeSpan.FromSeconds(1);

            await Task.Run(() =>
            {
                PressedKeysState result = null;
                try
                {
                    var PressedKeysMonitor = _monitorFactory.CreateGetPressedKeysMonitor(nemeio.CommandExecutor);
                    result = PressedKeysMonitor.GetPressedKeys();
                    _outputWriter.WriteObject(result);
                }
                catch (KeyboardException keyboardException)
                {
                    var output = new PressedKeysState(keyboardException.ErrorCode, new System.Collections.Generic.List<ushort>() { 0 });
                    _logger.LogError(this.GetType().Name, keyboardException);
                    _outputWriter.WriteObject(output);
                }
                catch (Exception exception)
                {
                    var errorMessage = $"GetPressedKeys failed";
                    _logger.LogError(errorMessage, exception);
                    _outputWriter.WriteObject(exception.Message);
                    throw new GetPressedKeysFailedException("GetPressedKeys failed", exception);
                }
            });
        }
    }
}
