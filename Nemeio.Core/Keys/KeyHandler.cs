using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Connection;
using Nemeio.Core.Keyboard.Keys;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Keyboard.Nemeios.Proxy;
using Nemeio.Core.Keys.Executors;
using Nemeio.Core.Layouts;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems.Hid;

namespace Nemeio.Core.Keys
{
    public class KeyHandler : IKeyHandler
    {
        private class PressableNemeioProxy : KeyboardProxy, IKeyPressHolder
        {
            private IKeyPressHolder _keysPressHolder;

            public PressableNemeioProxy(Keyboard.Nemeio nemeio)
                : base(nemeio)
            {
                _keysPressHolder = nemeio as IKeyPressHolder;
                _keysPressHolder.OnKeyPressed += KeysHolder_OnKeyPressed;
            }

            private void KeysHolder_OnKeyPressed(object sender, KeyboardKeyPressedEventArgs e)
            {
                OnKeyPressed?.Invoke(sender, e);
            }

            public event EventHandler<KeyboardKeyPressedEventArgs> OnKeyPressed;
        }

        private readonly ILogger _logger;
        private readonly ILayoutLibrary _library;
        private readonly ISystemHidInteractor _hidInteractor;
        private readonly KeystrokeInterpreter _keystrokeInterpreter;
        private readonly IKeyExecutorFactory _keyExecutorFactory;
        private readonly IKeyboardController _keyboardController;

        private PressableNemeioProxy _proxy;

        public KeyHandler(ILoggerFactory loggerFactory, ILayoutLibrary library, ISystemHidInteractor hidInteractor, IKeyExecutorFactory executorFactory, IKeyboardController keyboardController)
        {
            _logger = loggerFactory.CreateLogger<KeyHandler>();
            _library = library ?? throw new ArgumentNullException(nameof(library));
            _hidInteractor = hidInteractor ?? throw new ArgumentNullException(nameof(hidInteractor));
            _keyExecutorFactory = executorFactory ?? throw new ArgumentNullException(nameof(executorFactory));

            _keyboardController = keyboardController ?? throw new ArgumentNullException(nameof(keyboardController));
            _keyboardController.OnKeyboardConnected += KeyboardController_OnKeyboardConnected;
            _keyboardController.OnKeyboardDisconnecting += KeyboardController_OnKeyboardDisconnecting;

            _keystrokeInterpreter = new KeystrokeInterpreter();
        }

        ~KeyHandler()
        {
            _keyboardController.OnKeyboardConnected -= KeyboardController_OnKeyboardConnected;
            _keyboardController.OnKeyboardDisconnecting -= KeyboardController_OnKeyboardDisconnecting;
        }

        public async Task HandleAsync(INemeio nemeio, LayoutId id, IList<NemeioIndexKeystroke> keystrokes)
        {
            var layout = _library.Layouts.FirstOrDefault(x => x.LayoutId == id);
            if (layout == null)
            {
                _logger.LogWarning($"No layout found for id <{id}>. Bypass handle keys.");

                return;
            }

            var actions = _keystrokeInterpreter.GetActions(layout, keystrokes.ToArray());

            int receveidKeyStroke = actions.Count();
            if (receveidKeyStroke == 0)
            {
                _hidInteractor.StopSendKey();

                return;
            }

            var CtrlActions = (actions.Where(x => x.IsCtrl()).ToList());
            var OrderedActions = new List<KeySubAction>(CtrlActions);
            OrderedActions.AddRange(actions.Where(x => !x.IsCtrl()).ToList());
            var CTRLEX = _keyExecutorFactory.Create(nemeio, new List<KeySubAction>() { new KeySubAction("Ctrl", Enums.KeyActionType.Special) });
            //To ensure CTRL works we need to send it FIRST, then send the CTRL + Key sequence
            if (CtrlActions.Count > 0)
            {
                await CTRLEX.First().ExecuteAsync();
            }
            foreach (var executor in _keyExecutorFactory.Create(nemeio, OrderedActions))
            {
                await executor.ExecuteAsync();
            }
        }

        #region Events

        private void KeyboardController_OnKeyboardConnected(object sender, KeyboardStatusChangedEventArgs e)
        {
            var proxy = KeyboardProxy.CastTo<PressableNemeioProxy>(e.Keyboard);
            if (proxy != null)
            {
                _proxy = proxy;
                _proxy.OnKeyPressed += Runner_OnKeyPressed;
            }
        }

        private void KeyboardController_OnKeyboardDisconnecting(object sender, KeyboardStatusChangedEventArgs e)
        {
            if (_proxy != null && _proxy.Is(e.Keyboard))
            {
                //  Force stop interactor if keyboard disconnect
                _hidInteractor.StopSendKey();

                //  Unregister keyboard proxy
                _proxy.OnKeyPressed -= Runner_OnKeyPressed;
                _proxy = null;
            }
        }

        private async void Runner_OnKeyPressed(object sender, KeyboardKeyPressedEventArgs e)
        {
            await HandleAsync(_keyboardController.Nemeio, e.SelectedLayout, e.Keystrokes);
        }

        #endregion
    }
}
