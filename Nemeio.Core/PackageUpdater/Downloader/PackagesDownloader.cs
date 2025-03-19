using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Errors;
using Nemeio.Core.FileSystem;
using Nemeio.Core.PackageUpdater.Exceptions;
using Nemeio.Core.PackageUpdater.Informations;
using Nemeio.Core.Services;

namespace Nemeio.Core.PackageUpdater.Downloader
{
    public class PackagesDownloader : IPackagesDownloader
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private readonly IErrorManager _errorManager;
        private readonly IPackageUpdaterFileProvider _fileProvider;
        private readonly IFileSystem _fileSystem;
        private readonly IDocument _document;

        private int _fileCount;
        private int _fileIndex;

        public event EventHandler<PackagesDownloaderInProgressEventArgs> DownloadProgressChanged;

        public PackagesDownloader(ILoggerFactory loggerFactory, IErrorManager errorManager, IPackageUpdaterFileProvider fileProvider, IFileSystem fileSystem, IDocument document)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<PackagesDownloader>();
            _errorManager = errorManager ?? throw new ArgumentNullException(nameof(errorManager));
            _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _document = document ?? throw new ArgumentNullException(nameof(document));
        }

        public async Task Download(IList<DownloadablePackageInformation> informations)
        {
            if (informations == null)
            {
                throw new ArgumentNullException(nameof(informations));
            }

            if (informations.Count > 0)
            {
                _fileCount = informations.Count;
                _fileIndex = 0;

                foreach (var update in informations)
                {
                    var downloader = new PackageDownloader(_loggerFactory, _errorManager);

                    try
                    {
                        downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;

                        var updateContent = await downloader.DownloadAsync(update);

                        await SavePackageAsync(update, updateContent);

                        _fileIndex++;
                    }
                    finally
                    {
                        downloader.DownloadProgressChanged -= Downloader_DownloadProgressChanged;
                    }
                }
            }
        }

        /// <summary>
        /// Save a package on file system.
        /// </summary>
        /// <param name="package">Package to save</param>
        /// <param name="packageType">Is a application package (windows or mac) or firmware</param>
        /// <param name="packageContent">Package content, downloaded previously</parm>
        /// <exception cref="InvalidChecksumException">Thrown when file checksum is invalid. File will be deleted.</exception>
        /// <returns>Saved package file path</returns>
        private async Task<string> SavePackageAsync(DownloadablePackageInformation package, byte[] packageContent)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            var containerFolder = package.FindPackageContainerFolderPath(_document);

            _fileSystem.CreateDirectoryIfNotExists(containerFolder);

            var filePath = _fileProvider.GetFilePath(package);

            await _fileSystem.WriteAsync(filePath, packageContent);

            //  Check if file checksum is ok
            var checksumIsOk = CheckFileChecksum(package.Checksum, filePath);
            if (!checksumIsOk)
            {
                //  Check is not ok
                //  We delete corrupted file and stop current update

                _fileSystem.RemoveFileIfExists(filePath);

                throw new InvalidChecksumException();
            }

            return filePath;
        }

        private bool CheckFileChecksum(string checksum, string filePath)
        {
            var isValid = _fileSystem.FileChecksumIsValid(filePath, checksum);
            if (!isValid)
            {
                _logger.LogError($"{_errorManager.GetFullErrorMessage(ErrorCode.CoreUpdateDownloadFailed)}");
            }

            return isValid;
        }

        private void Downloader_DownloadProgressChanged(object sender, PackageDownloaderInProgressEventArgs e)
        {
            var args = new PackagesDownloaderInProgressEventArgs(_fileIndex, _fileCount, e.BytesIn, e.TotalBytes, e.Percent);

            DownloadProgressChanged?.Invoke(this, args);
        }
    }
}
