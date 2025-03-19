using System;

namespace Nemeio.Core.Tools.Watchers
{
    public interface IWatcher
    {
        event EventHandler OnChanged;
    }
}
