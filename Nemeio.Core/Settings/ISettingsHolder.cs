using System;

namespace Nemeio.Core.Settings
{
    public interface ISettingsHolder
    {
        event EventHandler OnSettingsUpdated;

        ISettings Settings { get; }
    }
}
