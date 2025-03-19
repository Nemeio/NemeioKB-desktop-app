using System;
using System.Collections.Generic;

namespace Nemeio.Tools.Testing.Update.Core.Reports
{
    public class TestReport : Report
    {
        public IList<UpdateTestReport> UpdateReports { get; private set; }

        public TestReport()
            : base(ReportStatus.Error, TimeSpan.Zero)
        {
            UpdateReports = new List<UpdateTestReport>();
        }

        public void AddReport(UpdateTestReport updateTestReport)
        {
            if (updateTestReport == null)
            {
                //  We don't allow to add null report
                return;
            }

            UpdateReports.Add(updateTestReport);
            ComputeStatus();
            ComputeDuration();
        }

        private void ComputeStatus()
        {
            var status = ReportStatus.Success;

            foreach (var report in UpdateReports)
            {
                if (report.Status == ReportStatus.Error)
                {
                    status = ReportStatus.Error;
                    break;
                }
            }

            Status = status;
        }

        private void ComputeDuration()
        {
            Duration = TimeSpan.Zero;
            foreach (var report in UpdateReports)
            {
                Duration += report.Duration;
            }
        }
    }
}
