using System.Collections.Generic;
using Nemeio.Core.DataModels;

namespace Nemeio.Core.Keyboard.KeyboardFailures
{
    public interface IKeyboardFailuresHolder
    {
        IList<KeyboardFailure> Failures { get; }
    }
}
