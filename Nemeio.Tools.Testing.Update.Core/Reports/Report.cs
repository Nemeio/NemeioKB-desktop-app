using System;

namespace Nemeio.Tools.Testing.Update.Core.Reports
{
    public abstract class Report
    {
        public ReportStatus Status { get; protected set; }
        public TimeSpan Duration { get; protected set; }

        public Report(ReportStatus status, TimeSpan duration)
        {
            Status = status;
            Duration = duration;
        }
    }
}
