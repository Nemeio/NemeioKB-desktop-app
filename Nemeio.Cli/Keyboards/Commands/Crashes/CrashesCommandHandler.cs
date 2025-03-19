using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Exceptions;
using Nemeio.Cli.Commands.Handlers.Keyboards;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.Crashes
{
    internal sealed class CrashesCommandHandler : KeyboardCommandHandler, ICrashesCommandHandler
    {
        private readonly IFileSystem _fileSystem;

        private CommandOption _directoryPath;

        public CrashesCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, IFileSystem fileSystem) 
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("crashes", "Download crashes from keyboard")) 
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public override void RegisterOptions(CommandLineApplication application)
        {
            _directoryPath = application.Option("-path |--path <path>", "Output directory path", CommandOptionType.SingleValue);
        }

        public override async Task ExecuteAsync()
        {
            var directoryPath = _directoryPath.Value();

            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new InvalidParameterException($"Directory parameter value cannot be empty or null");
            }

            try
            {
                var directory = _fileSystem.GetDirectory(directoryPath);

                var cancellationTokenSource = new CancellationTokenSource();
                var command = new CrashesCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource, directory);

                await command.ExecuteAsync();
            }
            catch (DirectoryNotFoundException exception)
            {
                _logger.LogError(exception, $"Download crashes failed");

                throw new InvalidParameterException($"Directory parameter is invalid", exception);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Download crashes failed");

                throw;
            }
        }
    }
}
