using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Managers;
using ObjCRuntime;
using WebKit;

namespace Nemeio.Mac.Models
{
    public class ConfiguratorNavigationDelegate : WKNavigationDelegate
    {
        private const int MaxRetryCount             = 5;
        private const string UrlErrorKey            = "NSErrorFailingURLKey";

        private TimeSpan _delayBeforeRetry = new TimeSpan(0, 0, 1);
        private int _retryCount = 0;

        private readonly ILogger _logger;
        private readonly ConfiguratorWebFileTransferDelegate _webFileTransferDelegate;

        public EventHandler NavigationFinished { get; set; }

        public ConfiguratorNavigationDelegate(ILoggerFactory loggerFactory, ILanguageManager languageManager)
        {
            _logger = loggerFactory.CreateLogger<ConfiguratorNavigationDelegate>();
            _webFileTransferDelegate = new ConfiguratorWebFileTransferDelegate(languageManager);
        }

        public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            webView.Hidden = false;

            NavigationFinished?.Invoke(this, null);
        }

        public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
        {
            var isDownload = _webFileTransferDelegate.IsDownloadAction(navigationAction);
            var policy = isDownload ? WKNavigationActionPolicy.Cancel : WKNavigationActionPolicy.Allow;

            decisionHandler(policy);

            if (isDownload)
            {
                _webFileTransferDelegate.DownloadDataOverRequest(navigationAction.Request);
            }
        }

        public override void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            _logger.LogError($"ConfiguratorNavigationDelegate.DidFailProvisionalNavigation: {error.LocalizedDescription}");

            if (_retryCount < MaxRetryCount)
            {
                Task.Delay(_delayBeforeRetry).Wait();

                _retryCount += 1;

                var urlRequestPath = error.UserInfo[UrlErrorKey] as NSUrl;
                if (urlRequestPath != null)
                {
                    _logger.LogInformation($"ConfiguratorNavigationDelegate.DidFailProvisionalNavigation: Retry {_retryCount}/{MaxRetryCount} for url <{urlRequestPath.AbsoluteString}>");

                    webView.LoadRequest(new NSUrlRequest(urlRequestPath));
                }
            }
            else
            {
                _logger.LogInformation("ConfiguratorNavigationDelegate.DidFailProvisionalNavigation: No more retry");
            }
        }
    }
}
