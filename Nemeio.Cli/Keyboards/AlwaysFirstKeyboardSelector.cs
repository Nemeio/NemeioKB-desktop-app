using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Keyboard.Communication;

namespace Nemeio.Cli.Keyboards
{
    internal sealed class AlwaysFirstKeyboardSelector : IKeyboardSelector
    {
        public Core.Keyboard.Keyboard SelectKeyboard(IEnumerable<Core.Keyboard.Keyboard> keyboards) => keyboards?.FirstOrDefault();
    }
}
