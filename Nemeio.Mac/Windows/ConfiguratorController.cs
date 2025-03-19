using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppKit;
using CoreFoundation;
using Foundation;
using Microsoft.Extensions.Logging;
using MvvmCross.Platform;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Managers;
using Nemeio.Core.Services;
using Nemeio.Core.Settings;
using Nemeio.Mac.Models;
using WebKit;

namespace Nemeio.Mac.Windows
{
    public partial class ConfiguratorController : NSWindowController, IWKUIDelegate
    {
        private readonly INemeioHttpService _nemeioHttpService;

        private ILogger _logger;
        private ILoggerFactory _loggerFactory;
        private ILanguageManager _languageManager;
        private ISettingsHolder _settingsHolder;
        private Action _didViewDismiss;

        public ConfiguratorController(IntPtr handle) 
            : base(handle) { }

        [Export("initWithCoder:")]
        public ConfiguratorController(NSCoder coder) 
            : base(coder) { }

        public ConfiguratorController(INemeioHttpService nemeioHttpService, Action didViewDismiss) 
            : base("Configurator")
        {
            _nemeioHttpService = nemeioHttpService;
            _didViewDismiss = didViewDismiss;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            _loggerFactory = Mvx.Resolve<ILoggerFactory>();
            _logger = _loggerFactory.CreateLogger<ConfiguratorController>();

            _languageManager = Mvx.Resolve<ILanguageManager>();
            _languageManager.LanguageChanged += LanguageManager_LanguageChanged;

            _settingsHolder = Mvx.Resolve<ISettingsHolder>();

            this.InitWindow();
        }

        public override void ShowWindow(NSObject sender)
        {
            base.ShowWindow(sender);

            WindowShown();
        }

        public void WindowShown()
        {
            _logger.LogInformation("ConfiguratorController.WindowShown");

            Window.WillClose += Window_WillClose;

            _nemeioHttpService.StartListenToRequests();

            var configuratorUrl = _nemeioHttpService.WebServer.ConfiguratorUrl;
            var urlRequest = new NSUrlRequest(new NSUrl(configuratorUrl));

            browser.LoadRequest(urlRequest);
        }

        public new Configurator Window => (Configurator)base.Window;

        private void InitWindow()
        {
            UpdateText();

            Window.Title = _languageManager.GetLocalizedValue(StringId.ConfiguratorTitle);

            var navigationDelegate = new ConfiguratorNavigationDelegate(_loggerFactory, _languageManager);
            navigationDelegate.NavigationFinished += NavigationFinished;

            browser.Hidden = true;
            browser.WeakUIDelegate = this;
            browser.NavigationDelegate = navigationDelegate;
            browser.AllowsBackForwardNavigationGestures = false;
            browser.AllowsLinkPreview = false;

            progressIndicator.StartAnimation(this);
        }

        private void LanguageManager_LanguageChanged(object sender, EventArgs e)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                UpdateText();
            });
        }

        [Export("webView:runOpenPanelWithParameters:initiatedByFrame:completionHandler:")]
        public void RunOpenPanel(WKWebView webView, WKOpenPanelParameters parameters, WKFrameInfo frame, Action<NSUrl[]> completionHandler)
        {
            var fileSelectorDialog = new NSOpenPanel();
            fileSelectorDialog.Title = _languageManager.GetLocalizedValue(StringId.FilePickerMessage);
            fileSelectorDialog.ShowsResizeIndicator = true;
            fileSelectorDialog.ShowsHiddenFiles = false;
            fileSelectorDialog.AllowsMultipleSelection = parameters.AllowsMultipleSelection;
            fileSelectorDialog.CanChooseFiles = true;
            fileSelectorDialog.CanChooseDirectories = parameters.AllowsDirectories;

            if (fileSelectorDialog.RunModal() == 1)
            {
                var urls = new List<NSUrl>() { fileSelectorDialog.Url }.ToArray();

                completionHandler(urls);
            }
            else
            {
                completionHandler(new NSUrl[0]);
            }
        }

        private void NavigationFinished(object sender, EventArgs e)
        {
            progressIndicator.Hidden = true;
            progressTextField.Hidden = true;
        }

        private void UpdateText()
        {
            Window.Title = _languageManager.GetLocalizedValue(StringId.ConfiguratorTitle);
            progressTextField.StringValue = _languageManager.GetLocalizedValue(StringId.CommonLoading);
        }

        private void Window_WillClose(object sender, EventArgs e)
        {
            Window.WillClose -= Window_WillClose;

            var webServerEnabled = _settingsHolder.Settings?.AutoStartWebServerSetting?.Value ?? false;
            if (!webServerEnabled)
            {
                Task.Run(async () =>
                {
                    await _nemeioHttpService?.StopListeningToRequestsAsync();
                });
            }

            _didViewDismiss?.Invoke();
        }
    }
}
