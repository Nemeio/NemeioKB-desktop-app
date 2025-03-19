using System;
using System.Threading.Tasks;
using Nemeio.Tools.Testing.Update.Core.Reports;

namespace Nemeio.Tools.Testing.Update.Core.Update.Tester
{
    public interface IUpdateTester
    {
        string Id { get; }
        Installer.Installer Starting { get; }
        Installer.Installer Target { get; }
        UpdateTestSteps Step { get; }
        TimeSpan Duration { get; }

        event EventHandler StepChanged;

        event EventHandler<double> OnVersionDownloadProgressChanged;
        event EventHandler OnVersionDownloadFinished;

        Task<UpdateTestReport> TestAsync();
    }
}
