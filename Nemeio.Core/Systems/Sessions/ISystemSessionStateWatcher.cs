using System;

namespace Nemeio.Core.Systems.Sessions
{
    public interface ISystemSessionStateWatcher : IDisposable
    {
        event EventHandler<SessionStateChangedEventArgs> StateChanged;
    }
}
