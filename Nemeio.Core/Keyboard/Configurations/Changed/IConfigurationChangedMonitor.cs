using System;

namespace Nemeio.Core.Keyboard.Configurations.Changed
{
    public interface IConfigurationChangedMonitor
    {
        event EventHandler<ConfigurationChangedEventArgs> ConfigurationChanged;
    }
}
