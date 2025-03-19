using System;

namespace Nemeio.Core.Tools
{
    public sealed class Timer : ITimer
    {
        private readonly System.Timers.Timer _nestedTimer;

        public TimeSpan Interval { get; private set; }

        public event EventHandler OnTimeElapsed;

        public Timer(TimeSpan timeSpan)
        {
            Interval = timeSpan;

            _nestedTimer = new System.Timers.Timer();
            _nestedTimer.AutoReset = true;
            _nestedTimer.Interval = Interval.TotalMilliseconds;
            _nestedTimer.Elapsed += NestedTimer_Elapsed;
        }

        ~Timer()
        {
            _nestedTimer.Elapsed -= NestedTimer_Elapsed;
        }

        public void Start() => _nestedTimer.Start();

        public void Stop() => _nestedTimer.Stop();

        private void NestedTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) => OnTimeElapsed?.Invoke(this, EventArgs.Empty);
    }
}
