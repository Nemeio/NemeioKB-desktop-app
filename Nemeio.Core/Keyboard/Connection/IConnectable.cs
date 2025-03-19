using Nemeio.Core.Keyboard.Communication;

namespace Nemeio.Core.Keyboard.Connection
{
    public interface IConnectable
    {
        CommunicationType CommunicationType { get; }
    }
}
