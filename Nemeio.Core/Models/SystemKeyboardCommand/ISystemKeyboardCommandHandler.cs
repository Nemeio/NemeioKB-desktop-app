using System.Collections.Generic;

namespace Nemeio.Core.Models.SystemKeyboardCommand
{
    public interface ISystemKeyboardCommandHandler
    {
        void RegisterCommand(SystemKeyboardCommand keyboardCommand);

        bool Handle(IList<string> keys);
    }
}
