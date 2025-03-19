using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Errors;
using Nemeio.Core.PackageUpdater.Exceptions;
using Nemeio.Core.PackageUpdater.Informations;

namespace Nemeio.Core.PackageUpdater.Downloader
{
    public class PackageDownloader
    {
        private static TimeSpan DefaultRequestTimeout = new TimeSpan(0, 0, 30);

        private readonly ILogger _logger;
        private readonly IErrorManager _errorManager;

        private System.Timers.Timer _timeoutTimer;

        private SemaphoreSlim _downloadSemaphore;
        private bool _isTimeout;
        private bool _downloadStarted;
        private TaskCompletionSource<byte[]> _downloadTask;

        public event EventHandler<PackageDownloaderInProgressEventArgs> DownloadProgressChanged;

        public PackageDownloader(ILoggerFactory loggerFactory, IErrorManager errorManager)
        {
            _downloadSemaphore = new SemaphoreSlim(1, 1);
            _logger = loggerFactory.CreateLogger<PackageDownloader>();
            _errorManager = errorManager ?? throw new ArgumentNullException(nameof(errorManager));

            _isTimeout = false;
            _downloadStarted = false;

            _timeoutTimer = new System.Timers.Timer();
            _timeoutTimer.Elapsed += TimeoutTimer_Elapsed;
            _timeoutTimer.Interval = DefaultRequestTimeout.TotalMilliseconds;
        }

        /// <summary>
        /// Allow to download package from server
        /// </summary>
        /// <param name="package">Package to download from server</param>
        /// <exception cref="PackageDownloaderTimeoutException">Timeout raised</exception>
        /// <returns></returns>
        public Task<byte[]> DownloadAsync(DownloadablePackageInformation package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            _downloadSemaphore.Wait();

            using (var client = new WebClient())
            {
                _downloadStarted = true;

                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
                client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(Client_DownloadDataCompleted);

                _timeoutTimer.Start();

                client.DownloadDataAsync(package.Url);

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
                if (_downloadSemaphore.CurrentCount == 0)
                {
                    _downloadSemaphore.Release();
                }

                _downloadStarted = false;

                _downloadTask.TrySetCanceled();
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _timeoutTimer?.Stop();

            var bytesIn = e.BytesReceived;
            var totalBytes = e.TotalBytesToReceive;
            var percentage = bytesIn * 100d / totalBytes;

            DownloadProgressChanged?.Invoke(
                this,
                new PackageDownloaderInProgressEventArgs(bytesIn, totalBytes, percentage)
            );

            try
            {
                _timeoutTimer?.Start();
            }
            //  Object Timer doesn't have IsDisposed property
            //  We can only catch the exception when occur
            catch (NullReferenceException) { }
            catch (ObjectDisposedException) { }
        }

        private void Client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (_isTimeout)
            {
                _downloadTask.TrySetException(new PackageDownloaderTimeoutException());

                return;
            }

            if (e.Cancelled)
            {
                _downloadTask.TrySetCanceled();

                return;
            }

            if (e.Error != null)
            {
                _downloadTask.TrySetException(e.Error);

                return;
            }

            _downloadTask.TrySetResult(e.Result);
        }

        private void TimeoutTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _isTimeout = true;
            _timeoutTimer.Elapsed -= TimeoutTimer_Elapsed;

            CancelDownload();
        }
    }
}
