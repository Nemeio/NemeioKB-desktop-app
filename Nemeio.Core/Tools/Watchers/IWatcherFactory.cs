namespace Nemeio.Core.Tools.Watchers
{
    public interface IWatcherFactory
    {
        IWatcher CreateFileWatcher(string forPath);
    }
}
