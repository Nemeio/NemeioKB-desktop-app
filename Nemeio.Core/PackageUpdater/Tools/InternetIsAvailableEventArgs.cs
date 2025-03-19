namespace Nemeio.Core.PackageUpdater.Tools
{
    public sealed class InternetIsAvailableEventArgs
    {
        public bool IsAvailable { get; private set; }

        public InternetIsAvailableEventArgs(bool isAvailable)
        {
            IsAvailable = isAvailable;
        }
    }
}
