using System;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Nemeio.Core.Systems.Sleep;

namespace Nemeio.Platform.Windows.Sleep
{
    public sealed class WinSleepModeAdapter : ISleepModeAdapter
    {
        private readonly ILogger _logger;

        public event EventHandler<SleepModeChangedEventArgs> OnSleepModeChanged;

        public WinSleepModeAdapter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<WinSleepModeAdapter>();

            SystemEvents.PowerModeChanged += OnPowerChange;
        }

        ~WinSleepModeAdapter()
        {
            SystemEvents.PowerModeChanged -= OnPowerChange;
        }

        private void OnPowerChange(object s, PowerModeChangedEventArgs e)
        {
            _logger.LogTrace($"Sleep mode changed <{e.Mode}>");

            switch (e.Mode)
            {
                case PowerModes.Resume:
                    RaiseSleepModeChanged(SleepMode.Awake);
                    break;
                case PowerModes.Suspend:
                    RaiseSleepModeChanged(SleepMode.Sleep);
                    break;
            }
        }

        private void RaiseSleepModeChanged(SleepMode mode) => OnSleepModeChanged?.Invoke(this, new SleepModeChangedEventArgs(mode));
    }
}
