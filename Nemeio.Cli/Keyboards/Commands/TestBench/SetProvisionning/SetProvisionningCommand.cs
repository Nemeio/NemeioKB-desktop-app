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

namespace Nemeio.Cli.Keyboards.Commands.TestBench.SetProvisionning
{
    internal sealed class SetProvisionningCommand : KeyboardCommand
    {
        private readonly SetProvisionningInput _setProvisionningInput;

        public SetProvisionningCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source, SetProvisionningInput setProvisionningInput)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source)
        {
            if (setProvisionningInput == null)
            {
                throw new ArgumentNullException(nameof(setProvisionningInput));
            }

            _setProvisionningInput = setProvisionningInput;
        }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            var timeout = TimeSpan.FromSeconds(1);

            await Task.Run(() =>
            {
                try
                {
                    var setProvisionningMonitor = _monitorFactory.CreateSetProvisionningMonitor(nemeio.CommandExecutor);
                    var result = setProvisionningMonitor.SetProvisionning(_setProvisionningInput.Serial, _setProvisionningInput.Mac);
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
                    var errorMessage = $"SetProvisionning  failed";
                    _outputWriter.WriteLine(exception.Message);
                    _logger.LogError(errorMessage, exception);
                    throw new SetProvisionningFailedException("SetProvisionning failed", exception);
                }
            });

        }
    }
}
