using System.Collections.Generic;

namespace Nemeio.Core.Keyboard.Communication.Watchers
{
    public interface IKeyboardWatcherFactory
    {
        IEnumerable<IKeyboardWatcher> CreateWatchers();
    }
}
