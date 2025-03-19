using System;

namespace Nemeio.Core.Systems.Applications
{
    public interface ISystemForegroundApplicationAdapter
    {
        event EventHandler<ApplicationChangedEventArgs> OnApplicationChanged;
        void Start();
        void Stop();
    }
}
