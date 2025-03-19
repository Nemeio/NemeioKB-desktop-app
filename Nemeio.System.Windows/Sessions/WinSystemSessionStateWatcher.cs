using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Nemeio.Core.Systems.Sessions;

namespace Nemeio.Platform.Windows.Sessions
{
    public class WinSystemSessionStateWatcher : ISystemSessionStateWatcher
    {
        private readonly ILogger _logger;

        public event EventHandler<SessionStateChangedEventArgs> StateChanged;

        public WinSystemSessionStateWatcher(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<WinSystemSessionStateWatcher>();

            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
        }

        public void Dispose()
        {
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            _logger.LogTrace($"Current session <{Process.GetCurrentProcess().SessionId}>");

            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                _logger.LogTrace("Win user session locked");

                StateChanged?.Invoke(this, new SessionStateChangedEventArgs(SessionState.Lock));
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                _logger.LogTrace("Win user session unlocked");

                StateChanged?.Invoke(this, new SessionStateChangedEventArgs(SessionState.Open));
            }
            else
            {
                _logger.LogWarning($"Win user session unsupported state <{e.Reason}>");
            }
        }
    }
}
