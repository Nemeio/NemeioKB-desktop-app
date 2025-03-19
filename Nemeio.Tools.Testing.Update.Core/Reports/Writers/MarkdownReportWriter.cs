using System;
using System.Threading.Tasks;
using Nemeio.Core.FileSystem;

namespace Nemeio.Tools.Testing.Update.Core.Reports.Writers
{
    public class MarkdownReportWriter : IReportWriter
    {
        private readonly IFileSystem _fileSystem;

        public MarkdownReportWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public async Task WriteAsync(string outputPath, TestReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }

            var outputPathWithName = $"{outputPath}.md";
            var content = $"#Report <{report.Status}>\n\n";

            foreach (var updateReport in report.UpdateReports)
            {
                content += $"* {updateReport.Starting} -> {updateReport.Target} [{updateReport.Duration}] : {updateReport.Status}\n";
            }

            await _fileSystem.WriteTextAsync(outputPathWithName, content);
        }
    }
}
