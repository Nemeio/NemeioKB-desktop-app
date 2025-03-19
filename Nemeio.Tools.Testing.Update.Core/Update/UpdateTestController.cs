using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nemeio.Core.FileSystem;
using Nemeio.Tools.Testing.Update.Core.Administrator;
using Nemeio.Tools.Testing.Update.Core.Reports;
using Nemeio.Tools.Testing.Update.Core.Reports.Writers;
using Nemeio.Tools.Testing.Update.Core.Update.Controller;
using Nemeio.Tools.Testing.Update.Core.Update.Tester;
using Nemeio.Tools.Testing.Update.Core.Update.Tester.Exceptions;
using Nemeio.Tools.Testing.Update.Core.Update.Tester.Settings;

namespace Nemeio.Tools.Testing.Update.Core.Update
{
    public class UpdateTestController : IUpdateTestController
    {
        private readonly IAdministratorChecker _administratorChecker;
        private readonly ITestSettingsLoader _settingsLoader;
        private readonly IFileSystem _fileSystem;
        private readonly IUpdateTesterFactory _updateTesterFactory;
        private readonly IReportWriterFactory _reportWriterFactory;

        public event EventHandler<TestStartedEventArgs> OnGlobalTestStarted;
        public event EventHandler<UpdateTestStartedEventArgs> OnTestStarted;
        public event EventHandler<UpdateTestEventArgs> OnTestFinished;
        public event EventHandler<TestFinishedEventArgs> OnGlobalTestFinished;

        public TestReport Report { get; private set; }

        public UpdateTestController(IAdministratorChecker administratorChecker, ITestSettingsLoader settingsLoader, IFileSystem fileSystem, IUpdateTesterFactory updateTesterFactory, IReportWriterFactory reportWriterFactory)
        {
            _administratorChecker = administratorChecker ?? throw new ArgumentNullException(nameof(administratorChecker));
            _settingsLoader = settingsLoader ?? throw new ArgumentNullException(nameof(settingsLoader));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _updateTesterFactory = updateTesterFactory ?? throw new ArgumentNullException(nameof(updateTesterFactory));
            _reportWriterFactory = reportWriterFactory ?? throw new ArgumentNullException(nameof(reportWriterFactory));
        }

        public async Task StartTestsAsync(string path)
        {
            //  First of all we need to check if we have admin's rights
            var runAsAdmin = _administratorChecker.RunAsAdministrator();
            if (!runAsAdmin)
            {
                throw new MissingAdministratorRightException();
            }

            var settings = _settingsLoader.LoadSettings(path);

            //  Create global output folder for tests
            _fileSystem.CreateDirectoryIfNotExists(settings.OutputPath);

            var testsReport = new TestReport();
            var testers = await _updateTesterFactory.CreateTesters(settings.EnvironmentName, settings.OutputPath, settings.ServerUri, settings.VersionRange);

            //  Order testers by starting version
            testers = testers
                .OrderBy(tester => tester.Starting.Version)
                .ToList();

            OnGlobalTestStarted?.Invoke(this, new TestStartedEventArgs(testers.Count()));

            foreach (var tester in testers)
            {
                OnTestStarted?.Invoke(this, new UpdateTestStartedEventArgs(tester));

                var testReport = await tester.TestAsync();

                testsReport.AddReport(testReport);

                OnTestFinished?.Invoke(this, new UpdateTestEventArgs(testReport));
            }

            Report = testsReport;

            await ExportReportAsync(settings);

            OnGlobalTestFinished?.Invoke(this, new TestFinishedEventArgs(Report));
        }

        private async Task ExportReportAsync(TestSettings settings)
        {
            const string reportFileName = "report";

            var writer = _reportWriterFactory.CreateWriter();
            var outputPath = Path.Combine(
                settings.OutputPath,
                reportFileName
            );

            await writer.WriteAsync(outputPath, Report);
        }
    }
}
