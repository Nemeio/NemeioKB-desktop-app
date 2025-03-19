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

namespace Nemeio.Cli.Keyboards.Commands.Configurations.Add
{
    internal sealed class AddLayoutCommandHandler : KeyboardCommandHandler, IAddLayoutCommandHandler
    {
        private readonly IFileSystem _fileSystem;

        private CommandOption _filePath;
        private CommandOption _factory;

        public AddLayoutCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, IFileSystem fileSystem)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("add", "Add layout to keyboard"))
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public override void RegisterOptions(CommandLineApplication application)
        {
            _filePath = application.Option("-path |--path <path>", "Layout's file path", CommandOptionType.SingleValue);
            _factory = application.Option("-factory", "Adds the layout as Factory Layout", CommandOptionType.NoValue);
        }
        public override async Task ExecuteAsync()
        {
            var filePath = _filePath.Value();
            var isFactoryLayout = _factory.HasValue() && _factory.Value()=="on";

            try
            {
                var file = _fileSystem.GetFile(filePath);
                var layoutFile = new CompressedLayoutFile(_fileSystem, file);
                var cancellationTokenSource = new CancellationTokenSource();
                
                var command = new AddLayoutCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource, layoutFile, isFactoryLayout);

                await command.ExecuteAsync();
            }
            catch (FileNotFoundException exception)
            {
                _logger.LogError(exception, $"AddLayoutCommand failed because file <{filePath}> not found");

                throw new InvalidParameterException();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"AddLayoutCommand failed");

                throw;
            }
        }
    }
}
