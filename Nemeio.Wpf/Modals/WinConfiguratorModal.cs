using System;
using System.Windows;
using Nemeio.Core.Managers;
using Nemeio.Core.Services;
using Nemeio.Core.Settings;
using Nemeio.Presentation.Modals;
using Nemeio.Wpf.Windows;

namespace Nemeio.Wpf.Modals
{
    public sealed class WinConfiguratorModal : IModalWindow
    {
        private Configurator _configurator;

        private readonly INemeioHttpService _httpService;
        private readonly ILanguageManager _languageManager;
        private readonly ISettingsHolder _settingsHolder;

        public bool IsOpen { get; private set; }

        public event EventHandler<OnClosingModalEventArgs> OnClosing;

        public WinConfiguratorModal(INemeioHttpService httpService, ILanguageManager languageManager, ISettingsHolder settingsHolder)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
            _languageManager = languageManager ?? throw new ArgumentNullException(nameof(languageManager));
            _settingsHolder = settingsHolder ?? throw new ArgumentNullException(nameof(settingsHolder));
        }

        public void Display()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                if (_configurator == null && !IsOpen)
                {
                    IsOpen = true;

                    _configurator = new Configurator(_httpService, _languageManager, _settingsHolder);
                    _configurator.Show();
                    _configurator.Closing += Configurator_Closing;
                }
            });
        }

        public void Focus()
        {
            if (_configurator != null && _configurator.IsLoaded)
            {
                if (_configurator.WindowState == WindowState.Minimized)
                {
                    _configurator.WindowState = WindowState.Maximized;
                }

                _configurator.Focus();
            }
        }

        private void Configurator_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is Configurator)
            {
                _configurator.Closing -= Configurator_Closing;
                _configurator = null;
                IsOpen = false;
                OnClosing?.Invoke(this, new OnClosingModalEventArgs(this));
            }
        }

        public void Close()
        {
            _configurator?.Close();

            IsOpen = false;

            OnClosing?.Invoke(this, new OnClosingModalEventArgs(this));
        }
    }
}
