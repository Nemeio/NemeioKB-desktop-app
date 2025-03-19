using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Cli.Keyboards.Commands;
using Nemeio.Cli.Keyboards.Commands.Update;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Commands.Handlers.Keyboards
{
    internal sealed class UpdateCommandHandler : KeyboardCommandHandler, IUpdateCommandHandler
    {
        private CommandOption _binaryFilePath;

        private readonly IFileSystem _fileSystem;

        public UpdateCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, IFileSystem fileSystem) 
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("update", "Start keyboard's update"))
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public override void RegisterOptions(CommandLineApplication application)
        {
            _binaryFilePath = application.Option("-binary |--binary <binary>", "Binary file path", CommandOptionType.SingleValue);
        }

        public override async Task ExecuteAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var command = new UpdateCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, _fileSystem, cancellationTokenSource, _binaryFilePath.Value());

            await command.ExecuteAsync();
        }
    }
}
