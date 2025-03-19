namespace Nemeio.Core.PackageUpdater.Downloader
{
    public class PackageDownloaderInProgressEventArgs
    {
        public long BytesIn { get; private set; }
        public long TotalBytes { get; private set; }
        public double Percent { get; private set; }

        public PackageDownloaderInProgressEventArgs(long bytesIn, long totalBytes, double percent)
        {
            BytesIn = bytesIn;
            TotalBytes = totalBytes;
            Percent = percent;
        }
    }
}
