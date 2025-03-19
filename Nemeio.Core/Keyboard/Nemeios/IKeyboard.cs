using Nemeio.Core.Keyboard.Connection;
using Nemeio.Core.Keyboard.Name;
using Nemeio.Core.Keyboard.SerialNumber;
using Nemeio.Core.Keyboard.State;
using Nemeio.Core.Keyboard.Version;
using Nemeio.Core.Tools.Stoppable;

namespace Nemeio.Core.Keyboard.Nemeios
{
    public interface IKeyboard : IAsyncStoppable, IConnectable, ISerialNumberHolder, IVersionHolder, INameHolder, IStateHolder
    {
        string Identifier { get; }
        System.Version ProtocolVersion { get; }
    }
}
