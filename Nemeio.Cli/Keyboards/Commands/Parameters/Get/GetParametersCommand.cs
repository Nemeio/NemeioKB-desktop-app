using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.Parameters.Get
{
    internal sealed class GetParametersCommand : KeyboardCommand
    {
        public GetParametersCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source) 
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source) { }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            await Task.Run(() => 
            {
                var parameterMonitor = _monitorFactory.CreateParametersMonitor(nemeio.Keyboard.ProtocolVersion, nemeio.CommandExecutor);

                try
                {
                    var parameters = parameterMonitor.GetParameters();
                    var output = new ParametersOutput(parameters);

                    _outputWriter.WriteObject(output);
                }
                catch (Exception exception)
                {
                    var errorMessage = $"Get parameters failed";

                    _logger.LogError(errorMessage, exception);

                    throw new GetParametersFailedException(errorMessage, exception);
                }
            });
        }
    }
}
