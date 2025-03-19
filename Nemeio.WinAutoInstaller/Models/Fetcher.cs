using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Nemeio.WinAutoInstaller.EventArgs;

namespace Nemeio.WinAutoInstaller.Models
{
    public enum FetchStep
    {
        GetUpdateInfo,
        DownloadUpdate
    }

    public class Fetcher
    {
        private const string KeyboardIdQuery            = "?keyboardId=";
        private const string ServerUrl                  = "https://karmeliet.witekio.com/api/updateInformation";
        private const string UpdateFile                 = "";

        private const string x64BitsPlatformName        = "x64";
        private const string x86BitsPlatformName        = "x86";

        private FetchStep _step;
        private Uri _downloadPath;
        private SoftwareInfo _currentUpdate;
        public WebFetcher _webFetcher;

        public event EventHandler<FetcherDownloadStartedEventArgs> DownloadStartedEvent;
        public event EventHandler<FetcherDownloadInProgressEventArgs> DownloadProgressChangedEvent;
        public event EventHandler<InstallerDownloadFinishedEventArgs> DownloadFinishedChangedEvent;

        public Fetcher()
        {
        }

        public async Task StartAsync()
        {
            _step = FetchStep.GetUpdateInfo;

            _webFetcher = new WebFetcher();
            _webFetcher.DownloadProgressChanged += WebFetcher_DownloadProgressChangedEventArgs;
            _webFetcher.DownloadFinished += WebFetcher_DownloadFinishedChangedEventArgs;
            _webFetcher.Start();

            await DownloadInfosAsync();
        }

        public void Stop()
        {
            Abort();
        }

        private void WebFetcher_DownloadProgressChangedEventArgs(object sender, FetcherDownloadInProgressEventArgs e)
        {
            switch (_step)
            {
                case FetchStep.GetUpdateInfo:
                    //  Nothing to do here
                    break;
                case FetchStep.DownloadUpdate:
                    RaiseDownloadProgressChanged(e.BytesIn, e.TotalBytes, e.Percent);
                    break;
                default:
                    throw new InvalidOperationException("Not supported step");
            }
        }

        private void WebFetcher_DownloadFinishedChangedEventArgs(object sender, WebFetcherDownloadFinishedEventArgs e)
        {
            if (e.ErrorCode != ErrorCode.WinAutoInstallerSuccess)
            {
                RaiseDownloadFinished(e.ErrorCode, null);
                return;
            }

            Task.Run(async () => 
            {
                switch (_step)
                {
                    case FetchStep.GetUpdateInfo:
                        await InfoFetchFinishedAsync(e.Data);
                        break;
                    case FetchStep.DownloadUpdate:
                        FileFetchFinished(e.Data);
                        break;
                    default:
                        throw new InvalidOperationException("Not supported step");
                }
            });
        }

        public void Abort()
        {
            _webFetcher?.Stop();
        }

        private async Task DownloadInfosAsync()
        {
            await _webFetcher.StartDownloadAsync(
                GetDownloadUri()
            );
        }

        private async Task InfoFetchFinishedAsync(byte[] data)
        {
            var jsonData = GetJsonData(data);
            var updateInfo = GetUpdateInfoFromJson(jsonData);
            var currentPlatform = Environment.Is64BitOperatingSystem ? x64BitsPlatformName : x86BitsPlatformName;

            try
            {
                _currentUpdate = updateInfo.Windows.Software.Items.First(x => x.Platform == currentPlatform);
                _step = FetchStep.DownloadUpdate;

                var packageName = Path.GetFileName(_currentUpdate.Url.ToString());
                _downloadPath = new Uri(Path.Combine(Path.GetTempPath(), packageName));
                await _webFetcher.StartDownloadAsync(new Uri(_currentUpdate.Url));

                RaiseDownloadStarted(_currentUpdate);
            }
            catch (InvalidOperationException)
            {
                _step = FetchStep.GetUpdateInfo;

                Logger.Instance.LogErrorCode(ErrorCode.WinAutoInstallerDownloadFailed);

                RaiseDownloadFinished(ErrorCode.WinAutoInstallerDownloadFailed, null);
            }
        }

        private string GetJsonData(byte[] data)
        {
            try
            {
                return Encoding.UTF8.GetString(data);
            }
            catch (Exception e)
            {
                Console.WriteLine("GetJsonData <{}>\n" + e.ToString());
                return null;
            }
        }

        private UpdateInfo GetUpdateInfoFromJson(string jsonData)
        {
            try
            {
                return UpdateInfo.FromJson(jsonData);
            }
            catch (Exception e)
            {
                var errorMessage = $"GetUpdateInfoFromJson <\n{jsonData}\n>\n" + e.ToString();
                Console.WriteLine(errorMessage);
                Logger.Instance.LogException(e, $"{errorMessage}");
                return null;
            }
        }

        private void FileFetchFinished(byte[] data)
        {
            var code = ErrorCode.WinAutoInstallerSuccess;

            try
            {
                //  Store file on temp directory
                var packageName = Path.GetFileName(_currentUpdate.Url.ToString());
                _downloadPath = new Uri(Path.Combine(Path.GetTempPath(), packageName));

                File.WriteAllBytes(_downloadPath.AbsolutePath, data);

                //  Need to check file checksum
                code = VerifyChecksum(_downloadPath, _currentUpdate.Checksum);
            }
            catch (Exception exception) when (exception is IOException || exception is DirectoryNotFoundException)
            {
                code = ErrorCode.WinAutoInstallerFileSystemWriteDenied;

                Logger.Instance.LogException(exception, $"[{(int)ErrorCode.WinAutoInstallerFileSystemWriteDenied}] Can't write on fileSystem");
            }

            RaiseDownloadFinished(code, _downloadPath);
        }

        public void DeleteTempFile()
        {
            if (_downloadPath != null)
            {
                try
                {
                    File.Delete(_downloadPath.AbsolutePath);
                }
                catch (Exception exception) when (exception is IOException || exception is DirectoryNotFoundException)
                {
                    //  Nothing to do here
                    Logger.Instance.LogException(exception, string.Empty);
                }
            }
        }

        private ErrorCode VerifyChecksum(Uri path, string checksum)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path.AbsolutePath))
                {
                    var hash = md5.ComputeHash(stream);
                    var fileChecksum = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

                    if (checksum.Equals(fileChecksum))
                    {
                        return ErrorCode.WinAutoInstallerSuccess;
                    }
                }
            }

            return ErrorCode.WinAutoInstallerInvalidChecksum;
        }

        private void RaiseDownloadProgressChanged(double bytesIn, double totalBytes, double percentage)
        {
            DownloadProgressChangedEvent?.Invoke(
                this,
                new FetcherDownloadInProgressEventArgs(bytesIn, totalBytes, percentage)
            );
        }

        private void RaiseDownloadFinished(ErrorCode code, Uri path)
        {
            DownloadFinishedChangedEvent?.Invoke(
                this,
                new InstallerDownloadFinishedEventArgs(code, path)
            );
        }

        private void RaiseDownloadStarted(SoftwareInfo software)
        {
            DownloadStartedEvent?.Invoke(
                this,
                new FetcherDownloadStartedEventArgs(software)
            );
        }

        private Uri GetDownloadUri()
        {
            var server = SettingsHelper.ReadSetting("ServerUrl");
            if (server == null)
            {
                server = ServerUrl;
            }

            var updateFile = SettingsHelper.ReadSetting("UpdateFile");
            if (updateFile == null)
            {
                updateFile = UpdateFile;
            }

            return new Uri($"{server}{updateFile}{GetCurrentKeyboardIdQuery()}");
        }

        private string GetCurrentKeyboardIdQuery()
        {
            var deviceId = WmiHelper.GetCurrentKeyboardIdentifier();
            return KeyboardIdQuery + deviceId;
        }
    }
}
