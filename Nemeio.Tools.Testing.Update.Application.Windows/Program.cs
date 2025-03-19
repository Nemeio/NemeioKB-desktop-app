using System;
using System.Diagnostics;
using Nemeio.Core.Versions;
using Nemeio.Tools.Testing.Update.Core.Reports;
using Nemeio.Tools.Testing.Update.Core.Update.Controller;
using Nemeio.Tools.Testing.Update.Core.Update.Installer;
using Nemeio.Tools.Testing.Update.Core.Update.Tester;

namespace Nemeio.Tools.Testing.Update.Application.Windows
{
    class Program
    {
        private static IUpdateTestController _updateTestController;
        private static IUpdateTester _currentTester;
        private static bool _isStartingDownload = true;

        static void Main(string[] args)
        {
            var applicationDelegate = new ApplicationDelegate();
            var application = new Application.Application(applicationDelegate);

            _updateTestController = application.IoC.GetService(typeof(IUpdateTestController)) as IUpdateTestController;
            _updateTestController.OnGlobalTestStarted += UpdateTestController_OnGlobalTestStarted;
            _updateTestController.OnGlobalTestFinished += UpdateTestController_OnGlobalTestFinished;
            _updateTestController.OnTestStarted += UpdateTestController_OnTestStarted;
            _updateTestController.OnTestFinished += UpdateTestController_OnTestFinished;

            var exitCode = application.Execute(args);

            Console.WriteLine(string.Empty);
            Console.WriteLine($"Execution finished with code <{exitCode}>");

            Console.ReadLine();
        }

        private static void UpdateTestController_OnTestStarted(object sender, UpdateTestStartedEventArgs e)
        {
            _isStartingDownload = true;
            _currentTester = e.Tester;
            _currentTester.OnVersionDownloadProgressChanged += CurrentTester_VersionDownloadProgressChanged;
            _currentTester.OnVersionDownloadFinished += CurrentTester_OnVersionDownloadFinished;
            _currentTester.StepChanged += CurrentTester_StepChanged;

            Console.WriteLine(string.Empty);
            Console.WriteLine($"Test started ({e.Tester.Id}) : {e.Tester.Starting.Version} -> {e.Tester.Target.Version} ============================");
            Console.WriteLine($"Testing... Please wait...");
            Console.WriteLine(string.Empty);
        }

        private static void CurrentTester_StepChanged(object sender, EventArgs e)
        {
            switch (_currentTester.Step)
            {
                case UpdateTestSteps.DownloadingStartingVersion:
                    Console.WriteLine(
                        BuildInstallerLine("Downloading", _currentTester.Starting)
                    );
                    break;
                case UpdateTestSteps.DownloadingTargetVersion:
                    Console.WriteLine(
                        BuildInstallerLine("Downloading", _currentTester.Target)
                    );
                    break;
                case UpdateTestSteps.InstallingStartingVersion:
                    Console.WriteLine(
                        BuildInstallerLine("Installation", _currentTester.Starting)
                    );
                    break;
                case UpdateTestSteps.InstallingTargetVersion:
                    Console.WriteLine(
                        BuildInstallerLine("Installation", _currentTester.Target)
                    );
                    break;
                case UpdateTestSteps.TryStartNemeioStartingVersion:
                    Console.WriteLine($"Try to start Nemeio with version <{_currentTester.Starting.Version}>");
                    break;
                case UpdateTestSteps.TryStartNemeioTargetVersion:
                    Console.WriteLine($"Try to start Nemeio with version <{_currentTester.Target.Version}>");
                    break;
                case UpdateTestSteps.UninstallTargetVersion:
                    Console.WriteLine(
                        BuildInstallerLine("Uninstall", _currentTester.Target)
                    );
                    break;
            }
        }
        
        private static void CurrentTester_OnVersionDownloadFinished(object sender, EventArgs e)
        {
            _isStartingDownload = false;
            Console.WriteLine(string.Empty);
        }

        private static void CurrentTester_VersionDownloadProgressChanged(object sender, double e) => NotifyDownload(GetCurrentVersion(), e);

        private static void UpdateTestController_OnTestFinished(object sender, UpdateTestEventArgs e)
        {
            _currentTester.OnVersionDownloadProgressChanged -= CurrentTester_VersionDownloadProgressChanged;
            _currentTester.OnVersionDownloadFinished -= CurrentTester_OnVersionDownloadFinished;
            _currentTester.StepChanged -= CurrentTester_StepChanged;
            _currentTester = null;

            Console.WriteLine(string.Empty);
            Console.ForegroundColor = e.Report.Status == ReportStatus.Error ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine($"Test finished with status <{e.Report.Status}> [{e.Report.Duration}]");
            Console.ResetColor();
        }

        private static void UpdateTestController_OnGlobalTestStarted(object sender, TestStartedEventArgs e) => Console.WriteLine($"Start testing ({e.TestCount}) ...");

        private static void UpdateTestController_OnGlobalTestFinished(object sender, TestFinishedEventArgs e)
        {
            Console.ForegroundColor = _updateTestController.Report.Status == ReportStatus.Error ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine(string.Empty);
            Console.WriteLine($"Test finished! <{_updateTestController.Report.Status}> [{_updateTestController.Report.Duration}]");
            Console.WriteLine(string.Empty);
            Console.WriteLine(string.Empty);
            Console.ResetColor();

            var updateReports = _updateTestController.Report.UpdateReports;
            if (updateReports != null && updateReports.Count > 0)
            {
                Console.WriteLine($"Report :");

                foreach (var report in updateReports)
                {
                    var message = $"- {report.Starting} -> {report.Target} : <{report.Status}> [{report.Duration}]";

                    Console.ForegroundColor = report.Status == ReportStatus.Error ? ConsoleColor.Red : ConsoleColor.Green;
                    Console.WriteLine(message);
                    Console.ResetColor();
                }
            }
        }

        private static string BuildInstallerLine(string text, Installer installer) => $"<{installer.Version}> : {text}";

        private static Version GetCurrentVersion() => _isStartingDownload ? _currentTester.Starting.Version : _currentTester.Target.Version;

        private static void NotifyDownload(Version version, double percent) => Console.Write($"\rDownloading ... <{version}> ({(int)percent}%)");
    }
}
