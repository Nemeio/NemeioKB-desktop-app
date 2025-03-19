using System;
using System.Threading.Tasks;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Connection;
using Nemeio.Core.Keyboard.FactoryReset;
using Nemeio.Core.Keyboard.Nemeios.Proxy;
using Nemeio.Core.Keyboard.State;
using Nemeio.Core.Services;

namespace Nemeio.Core.FactoryReset
{
    public class FactoryResetObserver : IFactoryResetObserver
    {
        private class FactoryResetNemeioProxy : KeyboardProxy, IFactoryResetHolder
        {
            private readonly IFactoryResetHolder _factoryResetHolder;

            public FactoryResetNemeioProxy(Keyboard.Nemeio nemeio) 
                : base(nemeio)
            {
                _factoryResetHolder = nemeio as IFactoryResetHolder;
            }

            public Task CancelFactoryResetAsync() => _factoryResetHolder.CancelFactoryResetAsync();

            public Task ConfirmFactoryResetAsync() => _factoryResetHolder.ConfirmFactoryResetAsync();

            public Task WantFactoryResetAsync() => _factoryResetHolder.WantFactoryResetAsync();
        }

        private readonly IKeyboardController _keyboardController;
        private readonly IApplicationService _applicationService;

        private bool _mustStopApplication = false;

        public FactoryResetObserver(IKeyboardController keyboardController, IApplicationService applicationService)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _keyboardController = keyboardController ?? throw new ArgumentNullException(nameof(keyboardController));
            _keyboardController.OnKeyboardDisconnecting += KeyboardController_OnKeyboardDisconnecting;
        }

        private void KeyboardController_OnKeyboardDisconnecting(object sender, KeyboardStatusChangedEventArgs e)
        {
            var proxy = KeyboardProxy.CastTo<FactoryResetNemeioProxy>(e.Keyboard);
            if (proxy != null && proxy.State == NemeioState.FactoryReset)
            {
                //  If we detect an unplugged keyboard which are on FactoryReset state,
                //  that mean factory reset is finished
                //  We can stop the application.

                _applicationService.StopApplication();
            }
        }
    }
}
