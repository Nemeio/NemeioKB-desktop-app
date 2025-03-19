using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Nemeio.Core;
using Nemeio.Core.JsonModels;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.PackageUpdater.Exceptions;
using Nemeio.Core.PackageUpdater.Informations;
using Nemeio.Core.Services;
using Nemeio.Core.Settings;
using OperatingSystem = Nemeio.Core.Services.OperatingSystem;

namespace Nemeio.Acl.HttpComm.PackageUpdater
{
    public class PackageUpdateRepository : IPackageUpdateRepository
    {
        private const string DefaultEnvironmentName = "master";
        private const string EndPoint = "https://nemeioupdateinquiry.azurewebsites.net/api/updateInformation{0}";

        private readonly IHttpService _httpService;
        private readonly IInformationService _informationService;
        private readonly ISettingsHolder _settings;

        public PackageUpdateRepository(IHttpService httpService, IInformationService informationService, ISettingsHolder settings)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
            _informationService = informationService ?? throw new ArgumentNullException(nameof(informationService));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<DownloadablePackageInformation> GetApplicationPackageInformationAsync()
        {
            try
            {
                var url = ComputeUpdateInformationUrlFromSettingsEnvironment();
                var packageInfoInDto = await _httpService.SendAsync<PackageUpdateManifestInDto>(url, HttpMethod.Get, null, null, true);

                return GetApplicationPackage(packageInfoInDto);
            }
            //  We want to catch every kind of error
            catch (Exception)
            {
                throw new RetrievePackageUpdateException();
            }
        }

        private DownloadablePackageInformation GetApplicationPackage(PackageUpdateManifestInDto inDto)
        {
            if (inDto == null)
            {
                throw new ArgumentNullException(nameof(inDto));
            }

            var operatingSystem = _informationService.GetOperatingSystem();
            switch (operatingSystem)
            {
                case OperatingSystem.Windows:
                    return GetWindowsPackage(inDto);
                case OperatingSystem.Osx:
                    return GetMacOSPackage(inDto);
                default:
                    throw new InvalidOperationException($"Not supported operating system <{operatingSystem}>");
            }
        }

        private DownloadablePackageInformation GetWindowsPackage(PackageUpdateManifestInDto inDto)
        {
            if (inDto == null)
            {
                throw new ArgumentNullException(nameof(inDto));
            }

            SoftwarePackageUpdateInDto windowsPackage = null;

            var architecture = _informationService.GetSystemArchitecture();
            switch (architecture)
            {
                case PlatformArchitecture.x64:
                    windowsPackage = inDto.Windows.Softwares.First(x => x.Platform == PlatformTypeInDto.x64);
                    break;
                case PlatformArchitecture.x86:
                    windowsPackage = inDto.Windows.Softwares.First(x => x.Platform == PlatformTypeInDto.x86);
                    break;
                default:
                    throw new InvalidOperationException($"<{architecture}> not supported");
            }

            return windowsPackage.ToDomainModel();
        }

        private DownloadablePackageInformation GetMacOSPackage(PackageUpdateManifestInDto inDto)
        {
            if (inDto == null)
            {
                throw new ArgumentNullException(nameof(inDto));
            }

            var software = inDto.MacOS.Softwares.First();

            return software.ToDomainModel();
        }

        private string ComputeUpdateInformationUrlFromSettingsEnvironment()
        {
            var environment = DefaultEnvironmentName;

            if (_settings.Settings != null)
            {
                environment = _settings.Settings.EnvironmentSetting.Value;
            }

            return GetUpdateInformationUri(environment).ToString();
        }

        private string GetUpdateInformationUri(string keyboardId)
        {
            var result = EndPoint;

            if (!string.IsNullOrEmpty(keyboardId))
            {
                var queryBuilder = new QueryBuilder();
                queryBuilder.Add("keyboardId", keyboardId);

                result = string.Format(EndPoint, queryBuilder.ToString());
            }
            else
            {
                result = string.Format(EndPoint, string.Empty);
            }

            return result;
        }
    }
}
