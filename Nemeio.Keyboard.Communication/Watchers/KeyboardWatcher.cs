using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Keyboard.Communication.Utils;
using Nemeio.Core.Keyboard.Communication.Watchers;
using CommKeyboard = Nemeio.Core.Keyboard.Keyboard;

namespace Nemeio.Keyboard.Communication.Watchers
{
    public abstract class KeyboardWatcher : IKeyboardWatcher, IDisposable
    {
        #region Variables

        private readonly object _keyboardLock = new object();
        protected readonly IKeyboardVersionParser _versionParser;

        #endregion

        #region Properties

        public IList<CommKeyboard> Keyboards { get; private set; }

        public event EventHandler OnKeyboardConnected;

        public event EventHandler<KeyboardDisconnectedEventArgs> OnKeyboardDisconnected;

        #endregion

        public KeyboardWatcher(IKeyboardVersionParser versionParser)
        {
            Keyboards = new List<CommKeyboard>();
            _versionParser = versionParser ?? throw new ArgumentNullException(nameof(versionParser));
        }

        public abstract void Dispose();

        protected void AddKeyboard(CommKeyboard keyboard)
        {
            if (keyboard == null)
            {
                //  We just do nothing if keyboard is null
                
                return;
            }

            lock(_keyboardLock)
            {
                if (!Keyboards.Any(kb => kb.Identifier == keyboard.Identifier))
                {
                    Keyboards.Add(keyboard);

                    RaiseKeyboardConnected();
                }
            }
        }

        protected void RemoveKeyboard(CommKeyboard keyboard)
        {
            if (keyboard == null)
            {
                //  We just do nothing if keyboard is null

                return;
            }

            lock(_keyboardLock)
            {
                if (Keyboards.Any(kb => kb.Identifier == keyboard.Identifier))
                {
                    var keyboards = Keyboards.Where(kb => kb.Identifier == keyboard.Identifier).ToList();

                    foreach (var selectedKeyboard in keyboards)
                    {
                        Keyboards.Remove(selectedKeyboard);

                        RaiseKeyboardDisconnected(selectedKeyboard);
                    }
                }
            }
        }

        protected void RaiseKeyboardConnected() => OnKeyboardConnected?.Invoke(this, EventArgs.Empty);

        protected void RaiseKeyboardDisconnected(CommKeyboard keyboard) => OnKeyboardDisconnected?.Invoke(this, new KeyboardDisconnectedEventArgs(keyboard));
    }
}
