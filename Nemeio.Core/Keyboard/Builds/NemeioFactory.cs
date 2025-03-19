using System;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Core.Keyboard.KeyboardFailures;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.Nemeios.Runner;
using Nemeio.Core.Keyboard.Nemeios.Updater;
using Nemeio.Core.Keyboard.Nemeios.VersionChecker;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Tools;
using Nemeio.Core.Tools.Retry;

namespace Nemeio.Core.Keyboard.Builds
{
    public class NemeioFactory : INemeioFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMonitorFactory _monitorFactory;
        private readonly IFrameParser _frameParser;
        private readonly IKeyboardCrashLogger _keyboardCrashLogger;
        private readonly IRetryHandler _retryHandler;
        private readonly IScreenFactory _screenFactory;

        public NemeioFactory(ILoggerFactory loggerFactory, IMonitorFactory monitorFactory, IFrameParser frameParser, IKeyboardCrashLogger crashLogger, IRetryHandler retryHandler, IScreenFactory screenFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _monitorFactory = monitorFactory ?? throw new ArgumentNullException(nameof(monitorFactory));
            _frameParser = frameParser ?? throw new ArgumentNullException(nameof(frameParser));
            _keyboardCrashLogger = crashLogger ?? throw new ArgumentNullException(nameof(crashLogger));
            _retryHandler = retryHandler ?? throw new ArgumentNullException(nameof(retryHandler));
            _screenFactory = screenFactory ?? throw new ArgumentNullException(nameof(screenFactory));
        }

        public Nemeio CreateRunner(Keyboard keyboard)
        {
            if (keyboard == null)
            {
                throw new ArgumentNullException(nameof(keyboard));
            }

            var commandExecutor = CreateCommandExecutor(keyboard);

            var keepAliveTimer = new Timer(new TimeSpan(0, 0, 1));
            var batteryTimer = new Timer(new TimeSpan(0, 1, 0));

            return new NemeioRunner(_loggerFactory, keyboard.Identifier, keyboard.ProtocolVersion, keyboard.Communication, commandExecutor, _monitorFactory, _keyboardCrashLogger, keepAliveTimer, batteryTimer, _retryHandler, _screenFactory);
        }

        public Nemeio CreateUpdater(Keyboard keyboard)
        {
            if (keyboard == null)
            {
                throw new ArgumentNullException(nameof(keyboard));
            }

            var commandExecutor = CreateCommandExecutor(keyboard);

            return new NemeioUpdater(_loggerFactory, keyboard.Identifier, keyboard.ProtocolVersion, keyboard.Communication, commandExecutor, _monitorFactory, _keyboardCrashLogger, _retryHandler);
        }

        public Nemeio CreateVersionChecker(Keyboard keyboard)
        {
            if (keyboard == null)
            {
                throw new ArgumentNullException(nameof(keyboard));
            }

            var commandExecutor = CreateCommandExecutor(keyboard);

            return new NemeioVersionChecker(_loggerFactory, keyboard.Identifier, keyboard.ProtocolVersion, keyboard.Communication, commandExecutor, _monitorFactory, _keyboardCrashLogger, _retryHandler);
        }

        private IKeyboardCommandExecutor CreateCommandExecutor(Keyboard keyboard)
        {
            return new KeyboardCommandExecutor(_loggerFactory, keyboard.IO, _frameParser, keyboard.Identifier);
        }
    }
}
