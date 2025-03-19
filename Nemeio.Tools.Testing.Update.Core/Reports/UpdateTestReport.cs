using System;

namespace Nemeio.Tools.Testing.Update.Core.Reports
{
    public class UpdateTestReport : Report
    {
        public Version Starting { get; private set; }
        public Version Target { get; private set; }

        public UpdateTestReport(Version starting, Version target, ReportStatus status, TimeSpan duration)
            : base(status, duration)
        {
            Starting = starting ?? throw new ArgumentNullException(nameof(starting));
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }
    }
}
