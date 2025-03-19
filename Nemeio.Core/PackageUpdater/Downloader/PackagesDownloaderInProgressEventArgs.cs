namespace Nemeio.Core.PackageUpdater.Downloader
{
    public class PackagesDownloaderInProgressEventArgs
    {
        public int CurrentFileIndex { get; private set; }
        public int FileCount { get; private set; }
        public long BytesIn { get; private set; }
        public long TotalBytes { get; private set; }
        public double Percent { get; private set; }

        public PackagesDownloaderInProgressEventArgs(int currentFileIndex, int fileCount, long bytesIn, long totalBytes, double percent)
        {
            CurrentFileIndex = currentFileIndex;
            FileCount = fileCount;
            BytesIn = bytesIn;
            TotalBytes = totalBytes;
            Percent = percent;
        }
    }
}
