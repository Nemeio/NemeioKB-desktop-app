using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Tools.Stoppable;
using Nemeio.Core.Tools.Watchers;

namespace Nemeio.Platform.Windows.Tools.Watchers
{
    public sealed class WinFileWatcher : Stoppable, IWatcher, IDisposable
    {
        private readonly string _filePath;
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;
        private readonly FileSystemWatcher _fileSystemWatcher;

        public event EventHandler OnChanged;

        public WinFileWatcher(ILoggerFactory loggerFactory, IFileSystem fileSystem, string filePath)
            : base()
        {
            _logger = loggerFactory.CreateLogger<WinFileWatcher>();
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

            if (!fileSystem.FileExists(filePath))
            {
                throw new ArgumentNullException($"<{filePath}> must exists");
            }

            _filePath = filePath;

            var folderPath = fileSystem.GetDirectoryName(filePath);
            var fileNameWithExtension = fileSystem.GetFileName(filePath);

            _fileSystemWatcher = new FileSystemWatcher(folderPath);
            _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess;
            _fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            _fileSystemWatcher.Filter = fileNameWithExtension;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation($"FileSystemWatcher_Changed <{e.ChangeType}>");

            if (_fileSystem.IsReady(_filePath))
            {
                _logger.LogInformation($"Something change on file <{_filePath}>");

                OnChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            if (_fileSystemWatcher != null)
            {
                _fileSystemWatcher.Dispose();
            }
        }
    }
}
