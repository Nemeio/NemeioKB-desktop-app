using System;
using Nemeio.Core.Services;
using Nemeio.Mac.Windows;
using Nemeio.Platform.Mac.Utils;
using Nemeio.Presentation.Modals;

namespace Nemeio.Mac.Modals
{
    public class MacConfiguratorModal : IModalWindow
    {
        private ConfiguratorController _configurator;

        private readonly INemeioHttpService _httpService;

        public bool IsOpen { get; private set; }

        public event EventHandler<OnClosingModalEventArgs> OnClosing;

        public MacConfiguratorModal(INemeioHttpService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public void Display()
        {
            DispatchQueueUtils.DispatchAsyncOnMainQueueIfNeeded(() =>
            {
                if (_configurator == null && !IsOpen)
                {
                    _configurator = new ConfiguratorController(_httpService, () =>
                    {
                        IsOpen = false;
                        _configurator = null;
                    });
                    _configurator.ShowWindow(null);
                    _configurator.Window.OrderFrontRegardless();

                    IsOpen = true;
                }
            });
        }

        public void Focus()
        {
            if (_configurator != null && _configurator.Window != null)
            {
                _configurator.Window.OrderFrontRegardless();
                _configurator.WindowShown();
            }
        }

        public void Close()
        {
            DispatchQueueUtils.DispatchAsyncOnMainQueueIfNeeded(() =>
            {
                _configurator.Close();
            });
        }
    }
}
