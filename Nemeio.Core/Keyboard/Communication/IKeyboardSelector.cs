using System.Collections.Generic;

namespace Nemeio.Core.Keyboard.Communication
{
    public interface IKeyboardSelector
    {
        Keyboard SelectKeyboard(IEnumerable<Keyboard> keyboards);
    }
}
