using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;

namespace Nemeio.Cli.Keyboards
{
    internal sealed class KeyboardAwaiter : IKeyboardAwaiter
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IFrameParser _frameParser;
        private readonly IKeyboardProvider _keyboarProvider;
        private readonly TaskCompletionSource<Core.Keyboard.Keyboard> _taskCompletionSource;

        private Nemeio _nemeio;
        private CancellationTokenSource _cancellation;

        public KeyboardAwaiter(ILoggerFactory loggerFactory, IKeyboardProvider keyboarProvider, IFrameParser frameParser)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _keyboarProvider = keyboarProvider ?? throw new ArgumentNullException(nameof(keyboarProvider));
            _frameParser = frameParser ?? throw new ArgumentNullException(nameof(frameParser));
            _taskCompletionSource = new TaskCompletionSource<Core.Keyboard.Keyboard>();

            _keyboarProvider.OnKeyboardConnected += KeyboarProvider_OnKeyboardConnected;
            _keyboarProvider.OnKeyboardDisconnected += KeyboarProvider_OnKeyboardDisconnected;
        }

        public async Task<Nemeio> WaitKeyboardAsync(CancellationTokenSource cancellation)
        {
            _cancellation = cancellation;
            _cancellation.Token.Register(CancellationTokenCancelled);

            //  First we check that a keyboard is already plugged
            var keyboard = _keyboarProvider.GetKeyboard(null);
            if (keyboard == null)
            {
                keyboard = await _taskCompletionSource.Task;
            }

            //  We don't need to listen OnConnected event
            //  because we already found keyboard
            _keyboarProvider.OnKeyboardConnected -= KeyboarProvider_OnKeyboardConnected;

            var commandExecutor = new KeyboardCommandExecutor(_loggerFactory, keyboard.IO, _frameParser, keyboard.Identifier);

            _nemeio = new Nemeio(keyboard, commandExecutor);

            return _nemeio;
        }

        private void KeyboarProvider_OnKeyboardConnected(object sender, EventArgs e)
        {
            var keyboard = _keyboarProvider.GetKeyboard(null);

            _taskCompletionSource.TrySetResult(keyboard);
        }

        private void KeyboarProvider_OnKeyboardDisconnected(object sender, Core.Keyboard.Communication.Watchers.KeyboardDisconnectedEventArgs e)
        {
            if (_nemeio != null && e.Identifier.Equals(_nemeio.Keyboard.Identifier))
            {
                _nemeio.IsDisconnected = true;
                _nemeio.Disconnected();
            }
        }

        public void Dispose()
        {
            if (_keyboarProvider != null)
            {
                _keyboarProvider.OnKeyboardConnected -= KeyboarProvider_OnKeyboardConnected;
                _keyboarProvider.OnKeyboardDisconnected -= KeyboarProvider_OnKeyboardDisconnected;
            }
        }

        private void CancellationTokenCancelled()
        {
            _taskCompletionSource.TrySetCanceled();
        }
    }
}
