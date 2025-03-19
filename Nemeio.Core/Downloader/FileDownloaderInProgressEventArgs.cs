namespace Nemeio.Core.Downloader
{
    public class FileDownloaderInProgressEventArgs
    {
        public long BytesIn { get; private set; }
        public long TotalBytes { get; private set; }
        public double Percent { get; private set; }

        public FileDownloaderInProgressEventArgs(long bytesIn, long totalBytes, double percent)
        {
            BytesIn = bytesIn;
            TotalBytes = totalBytes;
            Percent = percent;
        }
    }
}
