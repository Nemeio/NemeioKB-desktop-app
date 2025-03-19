using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.CommunicationMode;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts.Active;

namespace Nemeio.Core.Keyboard.Sessions.Strategies
{
    public sealed class NemeioLayoutEventStrategy : INemeioLayoutEventStrategy
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private readonly INemeio _nemeio;
        private readonly INemeioLayoutHolderProxy _proxy;
        private readonly IActiveLayoutChangeHandler _activeLayoutChangeHandler;

        public NemeioLayoutEventStrategy(ILoggerFactory loggerFactory, INemeio nemeio, INemeioLayoutHolderProxy proxy, IActiveLayoutChangeHandler activeLayoutChangeHandler)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _nemeio = nemeio ?? throw new ArgumentNullException(nameof(nemeio));
            _proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
            _activeLayoutChangeHandler = activeLayoutChangeHandler ?? throw new ArgumentNullException(nameof(activeLayoutChangeHandler));
            _logger = _loggerFactory.CreateLogger<NemeioLayoutEventStrategy>();
        }

        public async Task ConnectAsync()
        {
            _proxy.OnSelectedLayoutChanged += Proxy_OnSelectedLayoutChanged;
            _proxy.OnStateChanged += Proxy_OnStateChanged;

            await _activeLayoutChangeHandler.RequestResetHistoricAsync();
        }

        public async Task DisconnectAsync()
        {
            _proxy.OnSelectedLayoutChanged -= Proxy_OnSelectedLayoutChanged;
            _proxy.OnStateChanged -= Proxy_OnStateChanged;

            await _activeLayoutChangeHandler.RequestResetHistoricAsync();
        }

        public async Task SessionCloseAsync()
        {
            try
            {
                await _proxy.SetHidModeAsync();
            }
            catch (SetCommunicationModeFailedException exception)
            {
                _logger.LogError(exception, $"Can't set hid communication mode before disconnect");
            }
        }

        private async void Proxy_OnStateChanged(object sender, State.StateChangedEventArgs e)
        {
            await _activeLayoutChangeHandler.RequestHidSystemChangeAsync(_nemeio);
        }

        private async void Proxy_OnSelectedLayoutChanged(object sender, EventArgs e)
        {
            await _activeLayoutChangeHandler.RequestKeyboardSelectionChangeAsync(_nemeio);
        }
    }
}
