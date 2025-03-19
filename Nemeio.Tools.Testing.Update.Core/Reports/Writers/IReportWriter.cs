using System.Threading.Tasks;

namespace Nemeio.Tools.Testing.Update.Core.Reports.Writers
{
    public interface IReportWriter
    {
        Task WriteAsync(string outputPath, TestReport report);
    }
}
