using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Exceptions;
using Nemeio.Core.FileSystem;

namespace Nemeio.Cli.Package.Update.Commands.Read
{
    internal sealed class ReadUpdatePackageCommandHandler : CommandHandler, IReadUpdatePackageCommandHandler
    {
        private readonly IFileSystem _fileSystem;

        private CommandOption _filePath;

        public ReadUpdatePackageCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IFileSystem fileSystem) 
            : base(loggerFactory, outputWriter, new CommandInfo("readUpdatePackage", "Read manifest from update package")) 
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public override void RegisterOptions(CommandLineApplication application)
        {
            _filePath = application.Option("-path |--path <path>", "Layout's file path", CommandOptionType.SingleValue);
        }

        public override async Task ExecuteAsync()
        {
            var packageFile = _filePath.Value();

            if (string.IsNullOrEmpty(packageFile))
            {
                throw new InvalidParameterException($"File package path cannot be null or empty");
            }

            try
            {
                var file = _fileSystem.GetFile(packageFile);

                var cancellationTokenSource = new CancellationTokenSource();
                var command = new ReadUpdatePackageCommand(_loggerFactory, _outputWriter, cancellationTokenSource, file);

                await command.ExecuteAsync();
            }
            catch (FileNotFoundException exception)
            {
                _logger.LogError(exception, $"Read update package failed");

                throw new InvalidParameterException($"File <{packageFile}> not found", exception);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Read update package failed");

                throw;
            }
        }
    }
}
