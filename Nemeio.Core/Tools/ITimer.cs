using System;

namespace Nemeio.Core.Tools
{
    public interface ITimer
    {
        event EventHandler OnTimeElapsed;

        TimeSpan Interval { get; }

        void Start();
        void Stop();
    }
}
