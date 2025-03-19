using System;
using System.Net;
using System.Threading.Tasks;

namespace Nemeio.Core
{
    public class HttpClient : IHttpClient
    {
        private readonly WebClient _webClient;

        public HttpClient()
        {
            _webClient = new WebClient();
        }

        public Task DownloadFileTaskAsync(Uri url, string filename)
        {
            return _webClient.DownloadFileTaskAsync(url, filename);
        }

        public void Dispose()
        {
            _webClient.Dispose();
        }
    }
}
