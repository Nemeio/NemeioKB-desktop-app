using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Nemeio.Core.PackageUpdater.Exceptions;
using Nemeio.Core.PackageUpdater.Informations;
using Nemeio.Core.Services;

namespace Nemeio.Core.PackageUpdater.Strategies
{
    public class InstallApplicationStrategy : IInstallStrategy
    {
        private readonly DownloadablePackageInformation _packageInformation;
        private readonly IPackageUpdaterFileProvider _fileProvider;
        private readonly IApplicationService _applicationService;

        public InstallApplicationStrategy(DownloadablePackageInformation packageInformation, IPackageUpdaterFileProvider fileProvider, IApplicationService applicationService)
        {
            _packageInformation = packageInformation;
            _fileProvider = fileProvider;
            _applicationService = applicationService;
        }

        public async Task InstallAsync()
        {
            await Task.Yield();

            var filePath = _fileProvider.GetUpdateFilePath(_packageInformation);

            if (!File.Exists(filePath))
            {
                throw new InstallFailedException(InstallFailReason.FileNotFound);
            }

            try
            {
                var installProcess = new Process();
                installProcess.StartInfo.FileName = filePath;

                var startedSuccessfully = installProcess.Start();
                if (!startedSuccessfully)
                {
                    throw new InstallFailedException(InstallFailReason.LaunchFailed);
                }

                _applicationService.StopApplication();
            }
            catch (Exception)
            {
                throw new InstallFailedException(InstallFailReason.Unknown);
            }
        }
    }
}
