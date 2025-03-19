using System;
using Microsoft.Extensions.Logging;
using Nemeio.Core.FileSystem;

namespace Nemeio.Core.Tools.Watchers
{
    public abstract class WatcherFactory : IWatcherFactory
    {
        protected readonly ILoggerFactory _loggerFactory;
        protected readonly IFileSystem _fileSystem;

        public WatcherFactory(ILoggerFactory loggerFactory, IFileSystem fileSystem)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public abstract IWatcher CreateFileWatcher(string forPath);
    }
}
