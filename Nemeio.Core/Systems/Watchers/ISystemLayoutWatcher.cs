using System;
using System.Collections.Generic;

namespace Nemeio.Core.Systems.Watchers
{
    public interface ISystemLayoutWatcher
    {
        event EventHandler LayoutChanged;
        IEnumerable<string> Load();
    }
}
