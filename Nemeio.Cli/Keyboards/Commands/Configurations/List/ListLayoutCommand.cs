using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.Configurations.List
{
    internal sealed class ListLayoutCommand : KeyboardCommand
    {
        public ListLayoutCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source) 
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source) { }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            var getLayoutsMonitor = _monitorFactory.CreateGetLayoutsMonitor(nemeio.CommandExecutor);

            await Task.Run(() =>
            {
                try
                {
                    var ids= getLayoutsMonitor.AskLayoutIds();
                    var output = new LayoutsOutput(ids);

                    _outputWriter.WriteObject(output);
                }
                catch (Exception exception)
                {
                    var fetchLayoutsErrorMessage = "Fetch layout hashes failed";

                    _logger.LogError(exception, fetchLayoutsErrorMessage);

                    throw new ListLayoutFailedException(fetchLayoutsErrorMessage, exception);
                }
            });
        }
    }
}
