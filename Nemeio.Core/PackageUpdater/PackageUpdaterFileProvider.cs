using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Errors;
using Nemeio.Core.FileSystem;
using Nemeio.Core.PackageUpdater.Exceptions;
using Nemeio.Core.PackageUpdater.Informations;
using Nemeio.Core.Services;

namespace Nemeio.Core.PackageUpdater
{
    public class PackageUpdaterFileProvider : IPackageUpdaterFileProvider
    {
        private readonly ILogger _logger;
        private readonly IDocument _documentService;
        private readonly IErrorManager _errorManager;
        private readonly IFileSystem _fileSystem;

        public PackageUpdaterFileProvider(ILoggerFactory loggerFactory, IDocument documentService, IErrorManager errorManager, IFileSystem fileSystem)
        {
            _logger = loggerFactory.CreateLogger<PackageUpdaterFileProvider>();
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
            _errorManager = errorManager ?? throw new ArgumentNullException(nameof(errorManager));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }        

        /// <summary>
        /// Allow to retrieve a stored package path on file system
        /// </summary>
        /// <param name="package"></param>
        /// <exception cref="PackageNotFoundException">Not package found on file system. Maybe isn't downloaded yet.</exception>
        /// <returns></returns>
        public string GetUpdateFilePath(PackageInformation package)
        {
            var filePath = GetFilePath(package);

            if (_fileSystem.FileExists(filePath))
            {
                return filePath;
            }

            throw new PackageNotFoundException();
        }

        public string GetFilePath(PackageInformation package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            var filePath = package.FindPackageContainerFolderPath(_documentService);
            var packageName = package.GetPackageFileName();

            filePath = Path.Combine(filePath, packageName);
            filePath = Path.GetFullPath(filePath);

            return filePath;
        }
    }
}
