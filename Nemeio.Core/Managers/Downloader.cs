using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.DataModels;
using Nemeio.Core.Errors;

namespace Nemeio.Core.Managers
{
    public class Downloader : IDownloader
    {
        private readonly ILogger _logger;
        private readonly Updater _updater;
        private readonly IErrorManager _errorManager;

        private IHttpClient _client;

        public string LastError { get; private set; }

        public Downloader(ILoggerFactory loggerFactory, IHttpClient httpClient, Updater updater, IErrorManager errorManager)
        {
            _logger = loggerFactory.CreateLogger<Downloader>();
            _errorManager = errorManager;
            _client = httpClient;

            if (string.IsNullOrWhiteSpace(updater.InstallerPath))
            {
                _logger.LogError(
                    _errorManager.GetFullErrorMessage(ErrorCode.CoreUpdateUpdaterInvalid)    
                );

                throw new ArgumentException($"Invalid Updater data <{updater}>");
            }

            _updater = updater;
        }

        public async Task<bool> Download()
        {
            using (_client)
            {
                try
                {
                    LastError = null;
                    await _client.DownloadFileTaskAsync(_updater.Url, _updater.InstallerPath);
                    return CheckFileChecksum(_updater.InstallerPath);
                }
                catch (WebException exception)
                {
                    LastError = exception.ToString();

                    _logger.LogError(
                        exception, 
                        _errorManager.GetFullErrorMessage(ErrorCode.CoreUpdateDownloadFailed)
                    );

                    return false;
                }
            }
        }

        private bool CheckFileChecksum(string filePath)
        {
            var fileCheckSum = FileHelpers.CalculateMD5(filePath);
            if (fileCheckSum != _updater.Checksum)
            {
                LastError = $"Invalid checksum for <{_updater.Url}> found <{fileCheckSum}> expected <{_updater.Checksum}>";

                _logger.LogError(
                    $"{_errorManager.GetFullErrorMessage(ErrorCode.CoreUpdateDownloadFailed)}. {LastError}"
                );

                return false;
            }
            return true;
        }
    }
}
