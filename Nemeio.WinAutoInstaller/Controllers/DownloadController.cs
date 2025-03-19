using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nemeio.WinAutoInstaller.EventArgs;
using Nemeio.WinAutoInstaller.Models;

namespace Nemeio.WinAutoInstaller.Controllers
{
    public class DownloadController
    {
        private Fetcher _fetcher;
        private bool _cleanIsNeeded = true;

        public string CurrentVersion { get; private set; }
        public string CurrentArchitecture { get; private set; }

        public event EventHandler<InstallerDownloadProgressChangedEventArgs> DownloadProgressChanged;
        public event EventHandler<InstallerDownloadFinishedEventArgs> DownloadFinished;

        public async Task StartAsync()
        {
            _fetcher = new Fetcher();
            _fetcher.DownloadStartedEvent += Fetcher_DownloadStartedEvent;
            _fetcher.DownloadProgressChangedEvent += Fetcher_DownloadProgressChangedEvent;
            _fetcher.DownloadFinishedChangedEvent += Fetcher_DownloadFinishedChangedEvent;

            await _fetcher.StartAsync();
        }

        public void Stop()
        {
            if (_fetcher != null)
            {
                _fetcher.Abort();
                _fetcher.Stop();

                _fetcher.DownloadStartedEvent -= Fetcher_DownloadStartedEvent;
                _fetcher.DownloadProgressChangedEvent -= Fetcher_DownloadProgressChangedEvent;
                _fetcher.DownloadFinishedChangedEvent -= Fetcher_DownloadFinishedChangedEvent;

                if (_cleanIsNeeded)
                {
                    _fetcher.DeleteTempFile();
                }
            }
        }

        #region Events

        private void Fetcher_DownloadStartedEvent(object sender, EventArgs.FetcherDownloadStartedEventArgs e)
        {
            CurrentArchitecture = e.Software.Platform;
            CurrentVersion = e.Software.Version;
        }

        private void Fetcher_DownloadProgressChangedEvent(object sender, EventArgs.FetcherDownloadInProgressEventArgs e)
        {
            var percentValue = int.Parse(Math.Truncate(e.Percent).ToString());

            RaiseDownloadProgressChanged(percentValue);
        }

        private void Fetcher_DownloadFinishedChangedEvent(object sender, InstallerDownloadFinishedEventArgs e)
        {
            if (e.ErrorCode == ErrorCode.WinAutoInstallerSuccess)
            {
                _cleanIsNeeded = false;
            }

            RaiseDownloadFinished(e.ErrorCode, e.DownloadPath);
        }

        #endregion

        private void RaiseDownloadProgressChanged(int percent)
        {
            DownloadProgressChanged?.Invoke(
                this,
                new InstallerDownloadProgressChangedEventArgs(percent)
            );
        }

        private void RaiseDownloadFinished(ErrorCode code, Uri path)
        {
            DownloadFinished?.Invoke(
                this,
                new InstallerDownloadFinishedEventArgs(code, path)
            );
        }
    }
}
