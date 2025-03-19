using System;
using System.Threading.Tasks;
using Nemeio.Core.Tools;
using Nemeio.Core.Tools.Stoppable;
using Windows.Devices.Radios;
using Timer = System.Timers.Timer;

namespace Nemeio.Keyboard.Communication.Windows.Utils
{
    /// <summary>
    ///     In the case of a BLE connection / disconnection, several cases are to be managed: 
    ///     - Enable / Disable from the system
    ///     - Plugged in / Tore out a Bluetooth dongle
    ///     The system does not notify us when a dongle is plugged in or pulled out. The part taken for the moment is to pull on the Bluetooth status.
    /// </summary>
    public class WinBleStateChecker : Stoppable
    {
        public static readonly TimeSpan StandardCheckInterval = new TimeSpan(0, 0, 5);

        private readonly Radio _radio;
        private readonly Timer _timer;

        private RadioState _lastRadioState;

        private Action StateChangeCallback { get; set; }

        public WinBleStateChecker(Radio radio, Action callback, TimeSpan checkInterval = default(TimeSpan))
            : base(false)
        {
            _radio = radio;

            StateChangeCallback = callback;

            var interval = checkInterval == default(TimeSpan) ? StandardCheckInterval : checkInterval;

            _timer = new Timer();
            _timer.Interval = interval.TotalMilliseconds;
            _timer.Elapsed += Timer_Elapsed; ;
            _timer.AutoReset = true;
        }

        public void Start()
        {
            AliveState = AliveState.Starting;

            _timer.Start();

            AliveState = AliveState.Started;
        }

        public override void Stop()
        {
            _timer.Stop();

            base.Stop();
        }

        private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            await CheckStateAsync();
        }

        private async Task CheckStateAsync()
        {
            await Task.Yield();

            var state = _radio.State;
            if (state != _lastRadioState)
            {
                _lastRadioState = state;

                StateChangeCallback?.Invoke();
            }
        }
    }
}
