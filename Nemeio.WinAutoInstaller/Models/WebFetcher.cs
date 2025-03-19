using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Nemeio.WinAutoInstaller.EventArgs;

namespace Nemeio.WinAutoInstaller.Models
{
    public class WebFetcher
    {
        private static TimeSpan DefaultRequestTimeout = new TimeSpan(0, 0, 30);

        private System.Timers.Timer _timeoutTimer;

        private WebClient _client;
        private NetworkConnectivityChecker _connectivityChecker;
        private readonly SemaphoreSlim _downloadSemaphore;
        private bool _internetIsAvailable;
        private bool _isTimeout;

        public event EventHandler<FetcherDownloadInProgressEventArgs> DownloadProgressChanged;
        public event EventHandler<WebFetcherDownloadFinishedEventArgs> DownloadFinished;

        public WebFetcher()
        {
            _downloadSemaphore = new SemaphoreSlim(1, 1);
        }

        public void Start()
        {
            _isTimeout = false;
            _internetIsAvailable = true;

            _timeoutTimer = new System.Timers.Timer();
            _timeoutTimer.Elapsed += TimeoutTimer_Elapsed;
            _timeoutTimer.Interval = DefaultRequestTimeout.TotalMilliseconds;

            _client = new WebClient();
            _client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            _client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(Client_DownloadDataCompleted);
        }

        public void Stop()
        {
            StopDownload();

            _client.Dispose();
        }

        public async Task StartDownloadAsync(Uri url)
        {
            Logger.Instance.LogInformation($"Start download at <{url.AbsoluteUri}>");

            _downloadSemaphore.Wait();

            _connectivityChecker = new NetworkConnectivityChecker();
            _connectivityChecker.ConnectivityStatusChanged += ConnectivityChecker_ConnectivityStatusChanged;
            _connectivityChecker.Start();

            if (!await NetworkConnectivityChecker.InternetIsAvailable())
            {
                Logger.Instance.LogErrorCode(ErrorCode.WinAutoInstallerInternetNotAvailable);
                RaiseDownloadFinished(ErrorCode.WinAutoInstallerInternetNotAvailable, new byte[0]);
                return;
            }

            _timeoutTimer.Start();
            _client.DownloadDataAsync(url);
        }

        public void StopDownload()
        {
            _client.CancelAsync();

            if (_connectivityChecker != null)
            {
                _connectivityChecker.ConnectivityStatusChanged -= ConnectivityChecker_ConnectivityStatusChanged;
                _connectivityChecker.Stop();
                _connectivityChecker = null;
            }

            if (_downloadSemaphore != null && _downloadSemaphore.CurrentCount == 0)
            {
                _downloadSemaphore.Release();
            }
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _timeoutTimer?.Stop();

            var bytesIn         = double.Parse(e.BytesReceived.ToString());
            var totalBytes      = double.Parse(e.TotalBytesToReceive.ToString());
            var percentage      = bytesIn / totalBytes * 100;

            DownloadProgressChanged?.Invoke(
                this,
                new FetcherDownloadInProgressEventArgs(bytesIn, totalBytes, percentage)
            );

            _timeoutTimer?.Start();
        }

        private void Client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            StopDownload();

            if (_isTimeout)
            {
                Logger.Instance.LogErrorCode(ErrorCode.WinAutoInstallerServerTimeout);
                RaiseDownloadFinished(ErrorCode.WinAutoInstallerServerTimeout, null);
                return;
            }

            if (!_internetIsAvailable)
            {
                Logger.Instance.LogErrorCode(ErrorCode.WinAutoInstallerInternetNotAvailable);
                RaiseDownloadFinished(ErrorCode.WinAutoInstallerInternetNotAvailable, null);
                return;
            }

            if (e.Cancelled)
            {
                Logger.Instance.LogErrorCode(ErrorCode.WinAutoInstallerDownloadCancelByUser);
                RaiseDownloadFinished(ErrorCode.WinAutoInstallerDownloadCancelByUser, null);
                return;
            }

            if (e.Error != null)
            {
                Logger.Instance.LogException(e.Error, $"[{(int)ErrorCode.WinAutoInstallerDownloadFailed}] Download failed with error");
                RaiseDownloadFinished(ErrorCode.WinAutoInstallerDownloadFailed, null);
                return;
            }

            Logger.Instance.LogInformation($"Download finished successfully!");

            RaiseDownloadFinished(ErrorCode.WinAutoInstallerSuccess, e.Result);
        }

        private void ConnectivityChecker_ConnectivityStatusChanged(object sender, NetworkConnectivityEventArgs e)
        {
            _internetIsAvailable = e.IsAvailable;
            if (!_internetIsAvailable)
            {
                StopDownload();
            }
        }

        private void TimeoutTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _isTimeout = true;
            _timeoutTimer.Elapsed -= TimeoutTimer_Elapsed;

            StopDownload();
        }

        private void RaiseDownloadFinished(ErrorCode code, byte[] result)
        {
            DownloadFinished?.Invoke(
                this,
                new WebFetcherDownloadFinishedEventArgs(code, result)
            );
        }
    }
}
