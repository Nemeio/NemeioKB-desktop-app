using System;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Layouts.Active;

namespace Nemeio.Core.Systems.Layouts
{
    public sealed class SystemLayoutEventHandler : ISystemLayoutEventHandler
    {
        private readonly ISystem _system;
        private readonly IKeyboardController _keyboardController;
        private readonly IActiveLayoutChangeHandler _activeLayoutChangeHandler;

        public SystemLayoutEventHandler(ISystem system, IKeyboardController keyboardController, IActiveLayoutChangeHandler activeLayoutChangeHandler)
        {
            _system = system ?? throw new ArgumentNullException(nameof(system));
            _keyboardController = keyboardController ?? throw new ArgumentNullException(nameof(keyboardController));
            _activeLayoutChangeHandler = activeLayoutChangeHandler ?? throw new ArgumentNullException(nameof(activeLayoutChangeHandler));

            _system.OnForegroundApplicationChanged += System_OnForegroundApplicationChanged;
            _system.OnSelectedLayoutChanged += System_OnSelectedLayoutChanged;
            _system.OnSessionStateChanged += System_OnSessionStateChanged;
        }

        private async void System_OnSessionStateChanged(object sender, EventArgs e)
        {
            var nemeio = _keyboardController.Nemeio;

            await _activeLayoutChangeHandler.RequestSessionChangeAsync(nemeio, _system.SessionState);
        }

        private async void System_OnSelectedLayoutChanged(object sender, EventArgs e)
        {
            var nemeio = _keyboardController.Nemeio;

            await _activeLayoutChangeHandler.RequestHidSystemChangeAsync(nemeio);
        }

        private async void System_OnForegroundApplicationChanged(object sender, EventArgs e)
        {
            var nemeio = _keyboardController.Nemeio;

            await _activeLayoutChangeHandler.RequestForegroundApplicationChangeAsync(nemeio, _system.ForegroundApplication);
        }
    }
}
