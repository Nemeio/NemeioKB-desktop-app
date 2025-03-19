using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.Crashes
{
    internal sealed class CrashesCommand : KeyboardCommand
    {
        private readonly ILogger _logger;
        private readonly IDirectory _directory;

        public CrashesCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source, IDirectory directory) 
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source)
        {
            _logger = loggerFactory.CreateLogger<CrashesCommand>();
            _directory = directory ?? throw new ArgumentNullException(nameof(directory));
        }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            var versionMonitor = _monitorFactory.CreateVersionMonitor(nemeio.CommandExecutor);
            var crashMonitor = _monitorFactory.CreateKeyboardFailuresMonitor(nemeio.CommandExecutor);

            await Task.Run(() => 
            {
                try
                {
                    var versions = versionMonitor.AskVersions();
                    var failures = crashMonitor.AskKeyboardFailures();

                    var crashLogger = new CliKeyboardCrashLogger(_directory.Path);
                    crashLogger.WriteKeyboardCrashLog(versions, failures);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Get keyboard's crash failed");

                    throw;
                }
            });
        }
    }
}
