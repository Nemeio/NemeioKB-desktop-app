using System;
using System.Threading.Tasks;

namespace Nemeio.Core.Downloader
{
    public interface IFileDownloader
    {
        event EventHandler<FileDownloaderInProgressEventArgs> DownloadProgressChanged;
        Task<byte[]> DownloadAsync(Uri uri);
        void CancelDownload();
    }
}
