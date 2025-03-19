using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.Version
{
    internal sealed class VersionCommand : KeyboardCommand
    {
        public VersionCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source) 
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source) { }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            await Task.Run(() => 
            { 
                try
                {
                    var versionMonitor = _monitorFactory.CreateVersionMonitor(nemeio.CommandExecutor);
                    var versions = versionMonitor.AskVersions();

                    _logger.LogInformation($"Version fetch : {versions}");

                    var output = new VersionOutput(versions);

                    _outputWriter.WriteObject(output);
                }
                catch (Exception exception)
                {
                    var errorMessage = $"Fetch version failed";

                    _logger.LogError(errorMessage, exception);

                    throw new FetchVersionFailedException("Fetch version failed", exception);
                }
            });
        }
    }
}
