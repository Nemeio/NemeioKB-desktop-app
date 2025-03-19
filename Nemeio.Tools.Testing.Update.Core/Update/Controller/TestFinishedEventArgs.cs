using System;
using Nemeio.Tools.Testing.Update.Core.Reports;

namespace Nemeio.Tools.Testing.Update.Core.Update.Controller
{
    public class TestFinishedEventArgs
    {
        public TestReport Report { get; private set; }

        public TestFinishedEventArgs(TestReport report)
        {
            Report = report ?? throw new ArgumentNullException(nameof(report));
        }
    }
}
