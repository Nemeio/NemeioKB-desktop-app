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

namespace Nemeio.Cli.Keyboards.Commands.Parameters.Set
{
    internal sealed class SetParameterCommand : KeyboardCommand
    {
        private readonly string _json;

        public SetParameterCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source, string json)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentNullException(nameof(json));
            }

            _json = json;
        }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            var parameters = TryParseParameters();

            var parametersMonitor = _monitorFactory.CreateParametersMonitor(nemeio.Keyboard.ProtocolVersion, nemeio.CommandExecutor);

            await Task.Run(() => 
            { 
                try
                {
                    var keyboardParameters = parametersMonitor.GetParameters();

                    parameters.Patch(keyboardParameters);

                    parametersMonitor.SetParameters(keyboardParameters);
                }
                catch (Exception exception)
                {
                    var errorMessage = "Set parameters failed";

                    _logger.LogError(exception, errorMessage);

                    throw new SetParameterFailedException(errorMessage, exception);
                }
            });
        }

        private ParametersInput TryParseParameters()
        {
            try
            {
                var inputs = JsonConvert.DeserializeObject<ParametersInput>(_json);

                return inputs;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Parse parameters failed");

                throw new InvalidParameterException();
            }
        }
    }
}
