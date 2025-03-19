using System;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Frame;

namespace Nemeio.Cli.Keyboards.Awaiters
{
    internal sealed class KeyboardAwaiterFactory : IKeyboardAwaiterFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IKeyboardProvider _keyboardProvider;
        private readonly IFrameParser _frameParser;

        public KeyboardAwaiterFactory(ILoggerFactory loggerFactory, IKeyboardProvider keyboardProvider, IFrameParser frameParser)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _keyboardProvider = keyboardProvider ?? throw new ArgumentNullException(nameof(keyboardProvider));
            _frameParser = frameParser ?? throw new ArgumentNullException(nameof(frameParser));
        }

        public IKeyboardAwaiter Create()
        {
            var  awaiter = new KeyboardAwaiter(_loggerFactory, _keyboardProvider, _frameParser);

            return awaiter;
        }
    }
}
