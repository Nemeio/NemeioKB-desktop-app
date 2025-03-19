using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Keyboard.Communication.Watchers;

namespace Nemeio.Core.Keyboard.Communication
{
    public class KeyboardProvider : IKeyboardProvider, IDisposable
    {
        private readonly IList<IKeyboardWatcher> _keyboardWatchers;
        private readonly IKeyboardSelector _selector;
        private readonly IKeyboardWatcherFactory _keyboardWatcherFactory;

        public event EventHandler OnKeyboardConnected;
        public event EventHandler<KeyboardDisconnectedEventArgs> OnKeyboardDisconnected;

        public KeyboardProvider(IKeyboardWatcherFactory watcherFactory, IKeyboardSelector selector)
        {
            _keyboardWatcherFactory = watcherFactory ?? throw new ArgumentNullException(nameof(watcherFactory));
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));

            _keyboardWatchers = new List<IKeyboardWatcher>(_keyboardWatcherFactory.CreateWatchers());

            if (!_keyboardWatchers.Any())
            {
                throw new ArgumentException("Must be at least one watcher");
            }

            RegisterWatchers();
        }

        public Keyboard GetKeyboard(Func<Keyboard, bool> filter)
        {
            var keyboards = new List<Keyboard>();
            _keyboardWatchers.ForEach(watcher => keyboards.AddRange(watcher.Keyboards));

            if (filter != null)
            {
                keyboards = keyboards
                    .Where(filter)
                    .ToList();
            }

            var selectedKeyboard = _selector.SelectKeyboard(keyboards);

            return selectedKeyboard;
        }

        public void Dispose()
        {
            UnregisterWatchers();
        }

        private void RegisterWatchers()
        {
            _keyboardWatchers.ForEach((watcher) =>
            {
                watcher.OnKeyboardConnected += Watcher_OnKeyboardConnected;
                watcher.OnKeyboardDisconnected += Watcher_OnKeyboardDisconnected;
            });
        }

        private void UnregisterWatchers()
        {
            _keyboardWatchers.ForEach((watcher) =>
            {
                watcher.OnKeyboardConnected -= Watcher_OnKeyboardConnected;
                watcher.OnKeyboardDisconnected -= Watcher_OnKeyboardDisconnected;
            });
        }

        #region Events

        private void Watcher_OnKeyboardConnected(object sender, EventArgs e) => RaiseConnectedKeyboard();

        private void Watcher_OnKeyboardDisconnected(object sender, KeyboardDisconnectedEventArgs e) => RaiseDisconnectedKeyboard(e.Identifier);

        #endregion

        private void RaiseConnectedKeyboard() => OnKeyboardConnected?.Invoke(this, EventArgs.Empty);

        private void RaiseDisconnectedKeyboard(string identifier) => OnKeyboardDisconnected?.Invoke(this, new KeyboardDisconnectedEventArgs(identifier));
    }
}
