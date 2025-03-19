using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nemeio.Core.Services
{
    /// <summary>
    /// Interface to create http request on different supported OS.
    /// </summary>
    public interface IHttpService
    {
        Uri Resource { get; set; }

        /// <summary>
        /// Retrieve information from specific Url.
        /// Use Http Get method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource">Url to use</param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string resource);

        /// <summary>
        /// Post information to a specific Url.
        /// Use Http Post method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource">Url to use</param>
        /// <param name="data">Data to send</param>
        /// <returns></returns>
        Task<T> PostAsync<T>(string resource, object data);

        Task<T> PostAsync<T>(string resource, Stream stream, string contentType);

        Task<T> SendAsync<T>(string resource, HttpMethod httpMethod, object data);

        Task<T> SendAsync<T>(string resource, HttpMethod httpMethod, Stream stream = null, string contentType = null, bool bypassServerUri = false);

        Task<bool> Ping(string resource);
    }
}
