using Microsoft.Extensions.Logging;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Tools.Watchers;

namespace Nemeio.Platform.Mac.Tools.Watchers
{
    public class MacWatcherFactory : WatcherFactory
    {
        public MacWatcherFactory(ILoggerFactory loggerFactory, IFileSystem fileSystem)
            : base(loggerFactory, fileSystem) { }

        public override IWatcher CreateFileWatcher(string forPath) => new MacFileWatcher(_loggerFactory, _fileSystem, forPath);
    }
}
