using System;
using System.Threading.Tasks;
using System.Windows;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Managers;
using Nemeio.Core.Services;
using Nemeio.Core.Settings;
using Nemeio.Wpf.Helpers;
using Nemeio.Wpf.Helpers.Cef;
using Nemeio.Wpf.Helpers.Cef.Dialog;
using Nemeio.Wpf.Models;

namespace Nemeio.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for Configurator.xaml
    /// </summary>
    public partial class Configurator : Window
    {
        private readonly INemeioHttpService _nemeioHttpService;
        private readonly ILanguageManager _languageManager;
        private readonly ISettingsHolder _settingsHolder;

        public Configurator(INemeioHttpService nemeioHttpService, ILanguageManager languageManager, ISettingsHolder settingsHolder)
        {
            _nemeioHttpService = nemeioHttpService ?? throw new ArgumentNullException(nameof(settingsHolder));

            _languageManager = languageManager ?? throw new ArgumentNullException(nameof(settingsHolder));
            _languageManager.LanguageChanged += LanguageManager_LanguageChanged;

            _settingsHolder = settingsHolder ?? throw new ArgumentNullException(nameof(settingsHolder));

            InitializeComponent();
            InitWindows();
        }

        private void LanguageManager_LanguageChanged(object sender, EventArgs e)
        {
            UpdateWindowsText();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var enabled = _settingsHolder.Settings?.AutoStartWebServerSetting.Value ?? false;
            if (!enabled)
            {
                Task.Run(async () =>
                {
                    await _nemeioHttpService.StopListeningToRequestsAsync();
                });
            }
        }

        private void Browser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Browser.IsBrowserInitialized)
            {
                _nemeioHttpService.StartListenToRequests();

                var configuratorUrl = _nemeioHttpService.WebServer.ConfiguratorUrl;

                Browser.LoadingStateChanged += Browser_LoadingStateChanged;
                Browser.DownloadHandler = new CefSharpDownloadHandler();
                Browser.DialogHandler = new CefSharpDialogHandler();
                Browser.Load(configuratorUrl);
            }
        }

        private void Browser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                Dispatcher.Invoke(() =>
                {
                    Loader.Visibility = Visibility.Collapsed;
                });
            }
        }

        private void InitWindows()
        {
            Browser.MenuHandler = new CefMenuHandler();

            UpdateWindowsText();
        }

        private void UpdateWindowsText()
        {
            Dispatcher.Invoke(() =>
            {
                var lodingInProgressText = _languageManager.GetLocalizedValue(StringId.ConfiguratorButtonOpen);
                var windowTitle = _languageManager.GetLocalizedValue(StringId.ConfiguratorTitle);

                LoadingText.Text = lodingInProgressText;
                Title = windowTitle;
            });
        }
    }
}
