using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Cli.Keyboards.Commands.Configurations.Delete
{
    internal sealed class DeleteLayoutCommand : LayoutHashCommand
    {
        public DeleteLayoutCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source, string layoutHash) 
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source, layoutHash) { }

        public override async Task RunAsync(Nemeio nemeio, LayoutId id)
        {
            var deleteConfigurationMonitor = _monitorFactory.CreateDeleteConfigurationMonitor(nemeio.CommandExecutor);

            await Task.Run(() =>
            {
                try
                {
                    deleteConfigurationMonitor.Delete(id);
                }
                catch (KeyboardException exception)
                {
                    _logger.LogError(exception, $"Keyboard exception");

                    if (exception.ErrorCode == KeyboardErrorCode.ProtectedConfiguration)
                    {
                        throw new DeleteProtectedLayoutException();
                    }
                    else if (exception.ErrorCode == KeyboardErrorCode.NotFound)
                    {
                        throw new LayoutNotFoundException();
                    }
                    else
                    {
                        throw new DeleteLayoutFailedException();
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Unknown exception");

                    throw;
                }
            });
        }
    }
}
