using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Cli.Keyboards.Commands.Update;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Commands
{
    internal sealed class UpdateCommand : KeyboardCommand
    {
        private readonly IFileSystem _fileSystem;
        private readonly string _binaryFilePath;
        private readonly TaskCompletionSource<string> _taskCompletionSource;

        private bool _updateStarted = false;

        public UpdateCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, IFileSystem fileSystem, CancellationTokenSource source, string binaryFilePath) 
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _binaryFilePath = binaryFilePath;
            _taskCompletionSource = new TaskCompletionSource<string>();
        }

        public override async Task ApplyAsync(Keyboards.Nemeio nemeio)
        {
            if (!_fileSystem.FileExists(_binaryFilePath))
            {
                throw new InvalidOperationException($"File <{_binaryFilePath}> doesn't exists");
            }

            CancellationTokenSource.Token.Register(() => 
            {
                _taskCompletionSource.TrySetCanceled();
            });

            var updateMonitor = _monitorFactory.CreateUpdateMonitor(nemeio.CommandExecutor);
            var updateProgressMonitor = _monitorFactory.CreateUpdateProgressMonitor(nemeio.CommandExecutor);
            updateProgressMonitor.OnUpdateProgressChanged += UpdateProgressMonitor_OnUpdateProgressChanged;
            updateProgressMonitor.OnUpdateFailed += UpdateProgressMonitor_OnUpdateFailed;

            var binaryContent = await _fileSystem.ReadByteArrayAsync(_binaryFilePath);

            try
            {
                _logger.LogInformation($"Sending firmware to keyboard ...");

                //  We not wait end of this task
                _ = Task.Run(() => updateMonitor.SendFirmware(binaryContent), CancellationTokenSource.Token);

                _logger.LogInformation($"File sent to keyboard");
                _logger.LogInformation($"Updating ...");

                if (!CancellationTokenSource.IsCancellationRequested)
                {
                    await _taskCompletionSource.Task;

                    _logger.LogInformation($"Update finished!");
                }
            }
            //  We want to catch anykind of exception in this case
            catch (Exception exception)
            {
                var errorMessage = $"Send firmware to keyboard failed";

                _logger.LogError(exception, errorMessage);

                throw new UpdateKeyboardFailedException(errorMessage, exception);
            }
            finally
            {
                updateProgressMonitor.OnUpdateProgressChanged -= UpdateProgressMonitor_OnUpdateProgressChanged;
                updateProgressMonitor.OnUpdateFailed -= UpdateProgressMonitor_OnUpdateFailed;

                if (CancellationTokenSource.IsCancellationRequested)
                {
                    //  Previous task has been canceled
                    //  We can't process update information
                    //  We consider update failed

                    throw new CancelCommandException();
                }
            }
        }

        protected override void OnKeyboardDisconnected()
        {
            base.OnKeyboardDisconnected();

            if (_updateStarted)
            {
                _logger.LogInformation($"Keyboard has been disconnected");

                _taskCompletionSource.TrySetResult(string.Empty);
            }
            else
            {
                var exception = new KeyboardDisconnectedException();

                _logger.LogError(exception, $"Keyboard has been disconnected");

                _taskCompletionSource.TrySetException(exception);
            }
        }

        private void UpdateProgressMonitor_OnUpdateProgressChanged(object sender, Core.Keyboard.Updates.Progress.InProgressUpdateProgressEventArgs e)
        {
            _updateStarted = true;

            _logger.LogInformation($"Updating <{e.Component}> <{e.Progress}/100>");
        }

        private void UpdateProgressMonitor_OnUpdateFailed(object sender, Core.Keyboard.Updates.Progress.FailedUpdateProgressEventArgs e)
        {
            var exception = new InvalidOperationException($"Update failed with error code <{e.Error}>");

            _logger.LogError(exception, $"Update failed!");

            _taskCompletionSource.TrySetException(exception);
        }
    }
}
