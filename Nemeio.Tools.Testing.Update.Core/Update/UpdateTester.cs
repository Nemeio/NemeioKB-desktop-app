using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Nemeio.Core.Downloader;
using Nemeio.Core.FileSystem;
using Nemeio.Tools.Testing.Update.Core.Reports;
using Nemeio.Tools.Testing.Update.Core.Update.Installer;
using Nemeio.Tools.Testing.Update.Core.Update.Software;
using Nemeio.Tools.Testing.Update.Core.Update.Tester;

namespace Nemeio.Tools.Testing.Update.Core.Update
{
    public class UpdateTester : IUpdateTester
    {
        private readonly string _outputFolder;
        private readonly IFileSystem _fileSystem;
        private readonly IFileDownloader _fileDownloader;
        private readonly IInstallerExecutor _installerExecutor;
        private readonly IInstallerCheckerVersion _installerCheckerVersion;
        private readonly ISoftwareExecutor _softwareExecutor;

        public string Id { get; private set; }
        public Installer.Installer Starting { get; private set; }
        public Installer.Installer Target { get; private set; }
        public UpdateTestSteps Step { get; private set; }
        public TimeSpan Duration { get; private set; }

        public event EventHandler<double> OnVersionDownloadProgressChanged;
        public event EventHandler OnVersionDownloadFinished;
        public event EventHandler StepChanged;

        public UpdateTester(Installer.Installer starting, Installer.Installer target, string outputFolder, IFileSystem fileSystem, IFileDownloader fileDownloader, IInstallerExecutor installerExecutor, IInstallerCheckerVersion installerCheckerVersion, ISoftwareExecutor softwareExecutor)
        {
            Starting = starting ?? throw new ArgumentNullException(nameof(starting));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Id = ComputeId(Starting.Version, Target.Version);

            _outputFolder = outputFolder ?? throw new ArgumentNullException(nameof(outputFolder));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _fileDownloader = fileDownloader ?? throw new ArgumentNullException(nameof(fileDownloader));
            _installerExecutor = installerExecutor ?? throw new ArgumentNullException(nameof(installerExecutor));
            _installerCheckerVersion = installerCheckerVersion ?? throw new ArgumentNullException(nameof(installerCheckerVersion));
            _softwareExecutor = softwareExecutor ?? throw new ArgumentNullException(nameof(softwareExecutor));
        }

        public async Task<UpdateTestReport> TestAsync()
        {
            var status = ReportStatus.Error;
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            //  Create temp folder
            var testFolderPath = Path.Combine(_outputFolder, Id.ToString());
            _fileSystem.CreateDirectoryIfNotExists(testFolderPath);

            _fileDownloader.DownloadProgressChanged += FileDownloader_DownloadProgressChanged;

            //  Download each files
            try
            {
                ChangeStepAndNotify(UpdateTestSteps.DownloadingStartingVersion);
                var startingInstallerBinaryPath = await DownloadAndSaveBinary(testFolderPath, Starting);

                ChangeStepAndNotify(UpdateTestSteps.DownloadingTargetVersion);
                var targetInstallerBinaryPath = await DownloadAndSaveBinary(testFolderPath, Target);

                _fileDownloader.DownloadProgressChanged -= FileDownloader_DownloadProgressChanged;

                //  Execute each installation
                ChangeStepAndNotify(UpdateTestSteps.InstallingStartingVersion);
                var installStartingVersionSucceed = await InstallAndCheckVersion(startingInstallerBinaryPath, Starting.Version);
                if (installStartingVersionSucceed)
                {
                    ChangeStepAndNotify(UpdateTestSteps.TryStartNemeioStartingVersion);
                    var nemeioWorks = await TryStartNemeio();
                    if (nemeioWorks)
                    {
                        ChangeStepAndNotify(UpdateTestSteps.InstallingTargetVersion);
                        var installTargetVersionSucceed = await InstallAndCheckVersion(targetInstallerBinaryPath, Target.Version);
                        if (installTargetVersionSucceed)
                        {
                            ChangeStepAndNotify(UpdateTestSteps.TryStartNemeioTargetVersion);
                            nemeioWorks = await TryStartNemeio();
                            if (nemeioWorks)
                            {
                                ChangeStepAndNotify(UpdateTestSteps.UninstallTargetVersion);
                                var uninstallTargetVersionSucceed = await _installerExecutor.UninstallAsync(targetInstallerBinaryPath, Target.Version);
                                if (uninstallTargetVersionSucceed == InstallationStatus.Succeed)
                                {
                                    status = ReportStatus.Success;
                                }
                            }
                        }
                    }
                }
            }
            //  If something goes wrong we won't tests stop
            //  So we catch everything
            catch
            {
                //  Nothing to do
            }

            stopWatch.Stop();
            Duration = stopWatch.Elapsed;

            return new UpdateTestReport(Starting.Version, Target.Version, status, Duration);
        }

        private async Task<bool> InstallAndCheckVersion(string path, Version version)
        {
            //  Execute each installation
            var installationStatus = await InstallAsync(path, version);
            if (installationStatus == InstallationStatus.Succeed)
            {
                //  Starting version installation succeed
                //  Check installed version
                var isGoodVersion = await _installerCheckerVersion.IsVersion(version);

                return isGoodVersion;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> TryStartNemeio()
        {
            const string NemeioProcessName = "Nemeio";
            const string NemeioInstallationPath = @"C:\Program Files\Nemeio\Nemeio.exe";

            var status = await _softwareExecutor.RunAsync(NemeioInstallationPath, NemeioProcessName);
            if (status == LaunchStatus.Error)
            {
                return false;
            }

            status = await _softwareExecutor.ForceQuit(NemeioProcessName);
            if (status == LaunchStatus.Error)
            {
                return false;
            }

            return true;
        }

        private async Task<InstallationStatus> InstallAsync(string path, Version version)
        {
            var installationStatus = await _installerExecutor.InstallAsync(path, version);

            return installationStatus;
        }

        private async Task<string> DownloadAndSaveBinary(string testFolderPath, Installer.Installer installer)
        {
            var installerContent = await _fileDownloader.DownloadAsync(new Uri(installer.Url));
            
            OnVersionDownloadFinished?.Invoke(this, EventArgs.Empty);

            var installerBinaryPath = Path.Combine(testFolderPath, installer.Version.ToString());

            await _fileSystem.WriteAsync(installerBinaryPath, installerContent);

            return installerBinaryPath;
        }

        private void ChangeStepAndNotify(UpdateTestSteps step)
        {
            Step = step;
            StepChanged?.Invoke(this, EventArgs.Empty);
        }

        private void FileDownloader_DownloadProgressChanged(object sender, FileDownloaderInProgressEventArgs e) => OnVersionDownloadProgressChanged?.Invoke(this, e.Percent);

        private string ComputeId(Version start, Version target)
        {
            var value = $"{start}-{target}";
            var stringBuilder = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                var result = hash.ComputeHash(Encoding.UTF8.GetBytes(value));

                foreach (var val in result)
                {
                    stringBuilder.Append(val.ToString("x2"));
                }
            }

            return stringBuilder
                .ToString()
                .Substring(0, 16);
        }
    }
}
