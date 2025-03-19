using System;
using System.Threading.Tasks;
using Nemeio.Tools.Testing.Update.Core.Reports;

namespace Nemeio.Tools.Testing.Update.Core.Update.Controller
{
    public interface IUpdateTestController
    {
        event EventHandler<TestStartedEventArgs> OnGlobalTestStarted;
        event EventHandler<UpdateTestStartedEventArgs> OnTestStarted;
        event EventHandler<UpdateTestEventArgs> OnTestFinished;
        event EventHandler<TestFinishedEventArgs> OnGlobalTestFinished;

        TestReport Report { get; }

        Task StartTestsAsync(string path);
    }
}
