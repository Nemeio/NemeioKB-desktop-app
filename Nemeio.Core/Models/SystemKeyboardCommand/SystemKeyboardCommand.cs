using System.Collections.Generic;

namespace Nemeio.Core.Models.SystemKeyboardCommand
{
    public abstract class SystemKeyboardCommand
    {
        public abstract bool IsTriggered(IList<string> keys);

        public abstract void Execute();
    }
}
