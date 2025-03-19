using System;

namespace Nemeio.Core.Tools
{
    public interface IStoppable
    {
        bool Started { get; }

        event EventHandler OnStopRaised;
        void Stop();
    }
}
