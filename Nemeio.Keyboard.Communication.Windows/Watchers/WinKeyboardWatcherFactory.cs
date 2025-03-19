using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Communication.Utils;
using Nemeio.Core.Keyboard.Communication.Watchers;
using Nemeio.Core.Tools.Retry;

namespace Nemeio.Keyboard.Communication.Windows.Watchers
{
    public class WinKeyboardWatcherFactory : IKeyboardWatcherFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IRetryHandler _retryHandler;
        private readonly IKeyboardVersionParser _versionParser;

        public WinKeyboardWatcherFactory(ILoggerFactory loggerFactory, IRetryHandler retryHandler, IKeyboardVersionParser versionParser)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _retryHandler = retryHandler ?? throw new ArgumentNullException(nameof(retryHandler));
            _versionParser = versionParser ?? throw new ArgumentNullException(nameof(versionParser));
        }

        public IEnumerable<IKeyboardWatcher> CreateWatchers()
        {
            return new List<IKeyboardWatcher>()
            { 
                new WinSerialKeyboardWatcher(_loggerFactory, _retryHandler, _versionParser),
                new WinBluetoothLEKeyboardWatcher(_loggerFactory, _versionParser)
            };
        }
    }
}
