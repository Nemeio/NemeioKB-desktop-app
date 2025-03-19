using System;
using System.Threading.Tasks;

namespace Nemeio.Core
{
    public interface IHttpClient : IDisposable
    {
        Task DownloadFileTaskAsync(Uri url, string filename);
    }
}
