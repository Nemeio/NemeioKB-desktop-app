using System;
using Nemeio.Core.FileSystem;

namespace Nemeio.Tools.Testing.Update.Core.Reports.Writers
{
    public class ReportWriterFactory : IReportWriterFactory
    {
        private readonly IFileSystem _fileSystem;

        public ReportWriterFactory(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public IReportWriter CreateWriter()
        {
            return new MarkdownReportWriter(_fileSystem);
        }
    }
}
