using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.DataModels;
using Nemeio.Core.PackageUpdater.Exceptions;
using Nemeio.Core.PackageUpdater.Informations;
using Nemeio.Core.Services;

namespace Nemeio.Core.PackageUpdater
{
    public class PackageUpdateChecker : IPackageUpdateChecker
    {
        private readonly ILogger _logger;
        private readonly IInformationService _informationService;
        private readonly IPackageUpdateRepository _packageUpdateRepository;

        public PackageUpdateChecker(ILoggerFactory loggerFactory, IInformationService informationService, IPackageUpdateRepository packageUpdateRepository)
        {
            _logger = loggerFactory.CreateLogger<PackageUpdateChecker>();
            _packageUpdateRepository = packageUpdateRepository ?? throw new ArgumentNullException(nameof(packageUpdateRepository));
            _informationService = informationService ?? throw new ArgumentNullException(nameof(informationService));
        }

        /// <summary>
        /// Allow to check if a new application version is available online
        /// </summary>
        /// <exception cref="RetrievePackageUpdateException">Can't get information from server</exception>
        /// <exception cref="NoUpdateAvailableException">No update found</exception>
        /// <returns></returns>
        public async Task<DownloadablePackageInformation> ApplicationNeedUpdateAsync()
        {
            _logger.LogInformation($"Start check application update");

            var package = await _packageUpdateRepository.GetApplicationPackageInformationAsync();

            var packageVersion = new VersionProxy(package.Version);
            var applicationVersion = _informationService.GetApplicationVersion();

            if (applicationVersion.IsEqualTo(packageVersion) || applicationVersion.IsHigherThan(packageVersion))
            {
                _logger.LogInformation($"No update found for application");

                return null;
            }

            _logger.LogInformation($"New version is available for application update <{packageVersion}>");

            return package;
        }
    }
}
