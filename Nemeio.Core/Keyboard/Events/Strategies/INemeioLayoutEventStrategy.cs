using System.Threading.Tasks;

namespace Nemeio.Core.Keyboard.Sessions.Strategies
{
    public interface INemeioLayoutEventStrategy
    {
        Task ConnectAsync();
        Task DisconnectAsync();
        Task SessionCloseAsync();
    }
}
