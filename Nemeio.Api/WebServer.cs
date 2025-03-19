using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.Services;
using Nemeio.Core.Settings;

namespace Nemeio.Api
{
    public class WebServer : IWebServer
    {
        private static readonly string CertificateName = "nemeio";
        private static readonly string CertificateFilename = $"{CertificateName}.pfx";
        private static readonly string CertificatePassword = "89BZEQh6veQ2NMn";
        private static readonly string CertificateNamespace = "Nemeio.Api.Resources";

        private CancellationTokenSource _cancellationTokenSource;
        private ILogger _logger;
        private IWebHost _host;
        private ISettingsHolder _settingsHolder;

        // FIXME: HTTPS Temp disable
        // Not manage HTTPS on Mac at the moment
        // public bool IsSecure { get; } = !RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public bool IsSecure { get; } = false;

        public int Port { get; private set; }

        public string ConfiguratorUrl
        {
            get
            {
                var protocol = IsSecure ? "https" : "http";

                return $"{protocol}://localhost:{Port}";
            }
        }

        public WebServer(ILoggerFactory loggerFactory, ISettingsHolder settingsHolder)
        {
            _logger = loggerFactory.CreateLogger<WebServer>();
            _settingsHolder = settingsHolder;
            _settingsHolder.OnSettingsUpdated += _settingsHolder_OnSettingsUpdated;
        }

        private async void _settingsHolder_OnSettingsUpdated(object sender, EventArgs e)
        {
            await StopAsync();
        }

        public void Start()
        {
            if (_host != null)
            {
                _logger.LogWarning($"Webserver is already started");

                return;
            }

            try
            {
                _logger.LogInformation("WebServer.Start");
                _cancellationTokenSource = new CancellationTokenSource();

                Port = GetWebServerPort();

                _host = CreateWebHostBuilder().Build();

                _ = _host.RunAsync(_cancellationTokenSource.Token);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "WebServer.Start");
                _ = StopAsync();
            }
        }

        public async Task StopAsync()
        {
            if (_cancellationTokenSource != null)
            {
                _logger.LogInformation("WebServer.Stop");

                _cancellationTokenSource.Cancel();

                if (_host != null)
                {
                    await _host.StopAsync();

                    _host.Dispose();
                    _host = null;
                }
            }
        }

        private IWebHostBuilder CreateWebHostBuilder()
        {
            return new WebHostBuilder()
                .UseKestrel(options =>
                    {
                        options.Listen(IPAddress.Loopback, Port, listenOptions =>
                        {
                            if (IsSecure)
                            {
                                var certificate = LoadCertificate();
                                listenOptions.UseHttps(certificate);
                            }
                        });
                    })
                .UseStartup<ApiStartup>();
        }

        private X509Certificate2 LoadCertificate()
        {
            var assembly = typeof(ApiStartup).GetTypeInfo().Assembly;
            var embeddedFileProvider = new EmbeddedFileProvider(assembly, CertificateNamespace);
            var certificateFileInfo = embeddedFileProvider.GetFileInfo(CertificateFilename);

            using (var certificateStream = certificateFileInfo.CreateReadStream())
            {
                byte[] certificatePayload;
                using (var memoryStream = new MemoryStream())
                {
                    certificateStream.CopyTo(memoryStream);
                    certificatePayload = memoryStream.ToArray();
                }

                return new X509Certificate2(certificatePayload, CertificatePassword);
            }
        }

        private int FindAvailableIpPort()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();

            var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();

            return port;
        }

        private int GetWebServerPort()
        {
            const int minimumPortValue = 0;
            const int maximmumPortValue = 65535;

            var apiPort = _settingsHolder.Settings?.ApiPort.Value;
            if (apiPort.HasValue && apiPort.Value >= minimumPortValue && apiPort.Value <= maximmumPortValue)
            {
                //  We assume developer check port is available
                return apiPort.Value;
            }

            return FindAvailableIpPort();
        }

#if NETSTANDARD2_1
        /// <summary>
        /// Create the self signed certificate pfx file. Require net standard >=2.1 or net core >=2.0
        /// </summary>
        public static void CreateSelfSignedServerCertificate()
        {
            var sanBuilder = new SubjectAlternativeNameBuilder();
            sanBuilder.AddDnsName("localhost");
            sanBuilder.AddIpAddress(IPAddress.Loopback);
            sanBuilder.AddIpAddress(IPAddress.IPv6Loopback);

            using (var publicKey = RSA.Create())
            {
                var request = new System.Security.Cryptography.X509Certificates.CertificateRequest(
                    $"CN={CertificateName}",
                    publicKey,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);

                request.CertificateExtensions.Add(sanBuilder.Build());

                var certificate = request.CreateSelfSigned(
                    new DateTimeOffset(DateTime.Today),
                    new DateTimeOffset(DateTime.Today.AddMonths(1)));

                var bytes = certificate.Export(X509ContentType.Pfx, CertificatePassword);
                File.WriteAllBytes(@"D:\Sources\ldlc\b2047-ldlc-karmeliet-desktop-app\Nemeio.Api\Resources\nemeio.pfx", bytes);
            }
        }
#endif
    }
}
