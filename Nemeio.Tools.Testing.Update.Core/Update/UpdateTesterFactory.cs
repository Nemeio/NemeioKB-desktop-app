using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nemeio.Core.Downloader;
using Nemeio.Core.FileSystem;
using Nemeio.Tools.Testing.Update.Core.Update.Environment;
using Nemeio.Tools.Testing.Update.Core.Update.Installer;
using Nemeio.Tools.Testing.Update.Core.Update.Software;

namespace Nemeio.Tools.Testing.Update.Core.Update.Tester
{
    public class UpdateTesterFactory : IUpdateTesterFactory
    {
        private readonly IFileSystem _fileSystem;
        private readonly IFileDownloader _fileDownloader;
        private readonly IInstallerExecutor _installerExecutor;
        private readonly IInstallerCheckerVersion _installerChecker;
        private readonly IInstallerRepository _installerRepository;
        private readonly ISoftwareExecutor _softwareExecutor;

        public UpdateTesterFactory(IFileSystem fileSystem, IFileDownloader fileDownloader, IInstallerExecutor installerExecutor, IInstallerCheckerVersion checker, IInstallerRepository installerRepository, ISoftwareExecutor softwareExecutor)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _fileDownloader = fileDownloader ?? throw new ArgumentNullException(nameof(fileDownloader));
            _installerExecutor = installerExecutor ?? throw new ArgumentNullException(nameof(installerExecutor));
            _installerChecker = checker ?? throw new ArgumentNullException(nameof(checker));
            _installerRepository = installerRepository ?? throw new ArgumentNullException(nameof(installerRepository));
            _softwareExecutor = softwareExecutor ?? throw new ArgumentNullException(nameof(softwareExecutor));
        }

        public async Task<IEnumerable<IUpdateTester>> CreateTesters(string environment, string outputPath, Uri serverUri, UpdateVersionRange range)
        {
            var result = new List<IUpdateTester>();
            var updateEnvironment = EnvironmentUtils.FromString(environment);
            
            var installers = await _installerRepository.GetInstallers(serverUri, updateEnvironment);
            if (installers == null)
            {
                return result;
            }

            var validInstallers = installers
                .Where(x => x.Version >= range.Start && x .Version <= range.End);

            var higherInstaller = validInstallers
                .Aggregate((installerOne, installerTwo) => installerOne.Version > installerTwo.Version ? installerOne : installerTwo);

            foreach (var installer in validInstallers)
            {
                if (higherInstaller.Version != installer.Version)   
                {
                    var newTester = new UpdateTester(installer, higherInstaller, outputPath, _fileSystem, _fileDownloader, _installerExecutor, _installerChecker, _softwareExecutor);

                    result.Add(newTester);
                }
            }

            return result;
        }
    }
}
