using System;
using System.Threading;

namespace Nemeio.Core.Tools.Stoppable
{
    public enum AliveState
    {
        Starting,
        Started,
        Stopping,
        Stopped
    }

    public abstract class Stoppable : IStoppable
    {
        private long _started;

        public AliveState AliveState
        {
            get
            {
                var value = Interlocked.Read(ref _started);

                return (AliveState)value;
            }
            set
            {
                var newValue = (long)value;

                Interlocked.Exchange(ref _started, newValue);
            }
        }

        public bool Started => AliveState == AliveState.Started;

        public event EventHandler OnStopRaised;

        public Stoppable(bool autoStart = true)
        {
            if (autoStart)
            {
                AliveState = AliveState.Started;
            }
        }

        public virtual void Stop()
        {
            if (AliveState == AliveState.Starting || AliveState == AliveState.Started)
            {
                AliveState = AliveState.Stopped;
                RaiseStop();
            }
        }

        protected void RaiseStop() => OnStopRaised?.Invoke(this, EventArgs.Empty);

    }
}
