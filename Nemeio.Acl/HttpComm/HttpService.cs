using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Errors;
using Nemeio.Core.Extensions;
using Nemeio.Core.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nemeio.Acl.HttpComm
{
    public class HttpService : IHttpService
	{
		public static string JsonContentType => "application/json";

		public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver(),
			NullValueHandling = NullValueHandling.Ignore
		};

		private readonly HttpClient _httpClient;
		private readonly ILogger _logger;
		private readonly IErrorManager _errorManager;

		public Uri Resource { get; set; }

		public HttpService(ILoggerFactory loggerFactory, IErrorManager errorManager)
		{
			_httpClient = new HttpClient();
			_logger = loggerFactory.CreateLogger<HttpService>();
			_errorManager = errorManager;
		}

		public Task<T> GetAsync<T>(string resource)
		{
			return SendAsync<T>(resource, HttpMethod.Get);
		}

		public Task<T> PostAsync<T>(string resource, object data)
		{
			return SendAsync<T>(resource, HttpMethod.Post, data);
		}

		public Task<T> PostAsync<T>(string resource, Stream stream, string contentType)
		{
			return SendAsync<T>(resource, HttpMethod.Post, stream, contentType);
		}

		public async Task<T> SendAsync<T>(string resource, HttpMethod httpMethod, object data)
		{
			var str = JsonConvert.SerializeObject(data, JsonSerializerSettings);
			using (var stream = str.GetStream())
			{
				return await SendAsync<T>(resource, httpMethod,
					stream, JsonContentType);
			}
		}

		public async Task<T> SendAsync<T>(string resource, HttpMethod httpMethod, Stream stream = null, string contentType = null, bool bypassServerUri = false)
		{
			Uri requestUri = null;

			if (!bypassServerUri)
			{
				requestUri = new Uri(Resource.AbsoluteUri + resource);
			}
			else
			{
				requestUri = new Uri(resource);
			}

			HttpResponseMessage response = null;
			if (httpMethod == HttpMethod.Get)
			{
				response = await _httpClient.GetAsync(requestUri);
			}
			else if (httpMethod == HttpMethod.Post)
			{
				var streamContent = new StreamContent(stream);

				streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

				response = await _httpClient.PostAsync(requestUri, streamContent);
			}

			if (response == null)
			{
				_logger.LogError(_errorManager.GetFullErrorMessage(ErrorCode.AclHttpRequestFailed));

				throw new NotImplementedException();
			}

			if (!response.IsSuccessStatusCode)
			{
				throw new HttpRequestException(response.ReasonPhrase);
			}

			var responseStr = await response.Content.ReadAsStringAsync();
			
			try
			{
				var result = JsonConvert.DeserializeObject<T>(responseStr);
				return result;
			}
			catch (JsonException)
			{
				return default(T);
			}
		}

        public Task<bool> Ping(string resource)
        {
			return Task.Run(() => 
			{
				try
				{
					using (var pinger = new Ping())
					{
						var reply = pinger.Send(resource);

						return reply.Status == IPStatus.Success;
					}
				}
				catch (PingException)
				{
					return false;
				}
			});
        }
    }
}
