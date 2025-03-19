using Microsoft.Extensions.Logging;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Tools.Watchers;

namespace Nemeio.Platform.Windows.Tools.Watchers
{
    public sealed class WinWatcherFactory : WatcherFactory
    {
        public WinWatcherFactory(ILoggerFactory loggerFactory, IFileSystem fileSystem) 
            : base(loggerFactory, fileSystem) { }

        public override IWatcher CreateFileWatcher(string forPath) => new WinFileWatcher(_loggerFactory, _fileSystem, forPath);
    }
}
