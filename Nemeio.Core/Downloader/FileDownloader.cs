using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Nemeio.Core.Downloader
{
    public class FileDownloader : IFileDownloader, IDisposable
    {
        private static TimeSpan DefaultRequestTimeout = new TimeSpan(0, 0, 30);

        private readonly ILogger _logger;

        private System.Timers.Timer _timeoutTimer;

        private SemaphoreSlim _downloadSemaphore;
        private bool _isTimeout;
        private bool _downloadStarted;
        private TaskCompletionSource<byte[]> _downloadTask;
        private static Mutex _downloadProgressChangeMutex = new Mutex();

        public event EventHandler<FileDownloaderInProgressEventArgs> DownloadProgressChanged;

        public FileDownloader(ILoggerFactory loggerFactory)
        {
            _downloadSemaphore = new SemaphoreSlim(1, 1);
            _logger = loggerFactory.CreateLogger<FileDownloader>();
        }

        /// <summary>
        /// Allow to download package from server
        /// </summary>
        /// <param name="package">Package to download from server</param>
        /// <exception cref="PackageDownloaderTimeoutException">Timeout raised</exception>
        /// <returns></returns>
        public Task<byte[]> DownloadAsync(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            _downloadSemaphore.Wait();

            _isTimeout = false;
            _downloadStarted = false;

            if (_timeoutTimer != null)
            {
                _timeoutTimer.Dispose();
            }

            _timeoutTimer = new System.Timers.Timer();
            _timeoutTimer.Elapsed += TimeoutTimer_Elapsed;
            _timeoutTimer.Interval = DefaultRequestTimeout.TotalMilliseconds;

            using (var client = new WebClient())
            {
                _downloadStarted = true;

                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
                client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(Client_DownloadDataCompleted);
                
                _timeoutTimer.Start();

                client.DownloadDataAsync(uri);

                _downloadTask = new TaskCompletionSource<byte[]>();

                return _downloadTask.Task;
            }
        }

        /// <summary>
        /// Cancel current download if exists.
        /// </summary>
        public void CancelDownload()
        {
            if (_downloadStarted)
            {
                FinishWithCancel();
            }
        }

        public void Dispose()
        {
            if (_timeoutTimer != null)
            {
                _timeoutTimer.Dispose();
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _downloadProgressChangeMutex.WaitOne();

            _timeoutTimer?.Stop();

            var bytesIn = e.BytesReceived;
            var totalBytes = e.TotalBytesToReceive;
            var percentage = bytesIn * 100d / totalBytes;

            DownloadProgressChanged?.Invoke(
                this,
                new FileDownloaderInProgressEventArgs(bytesIn, totalBytes, percentage)
            );

            _downloadProgressChangeMutex.ReleaseMutex();
        }

        private void Client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (_isTimeout)
            {
                FinishWithException(new FileDownloaderTimeoutException());
            }
            else if (e.Cancelled)
            {
                FinishWithCancel();
            }
            else if (e.Error != null)
            {
                FinishWithException(e.Error);
            }
            else
            {
                FinishWithResult(e.Result);
            }
        }

        private void FinishWithResult(byte[] result)
        {
            _downloadTask?.TrySetResult(result);
            Finish();
        }

        private void FinishWithException(Exception exception)
        {
            _downloadTask?.TrySetException(exception);
            Finish();
        }

        private void FinishWithCancel()
        {
            _downloadTask?.TrySetCanceled();
            Finish();
        }

        private void Finish()
        {
            try
            {
                _downloadSemaphore.Release();
            }
            catch (SemaphoreFullException)
            {
                //  Nothing to do here
            }
            finally
            {
                _downloadStarted = false;
            }
        }

        private void TimeoutTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _isTimeout = true;
            _timeoutTimer.Elapsed -= TimeoutTimer_Elapsed;

            CancelDownload();
        }
    }
}
