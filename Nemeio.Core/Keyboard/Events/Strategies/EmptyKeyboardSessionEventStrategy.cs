using System.Threading.Tasks;

namespace Nemeio.Core.Keyboard.Sessions.Strategies
{
    public sealed class EmptyKeyboardSessionEventStrategy : INemeioLayoutEventStrategy
    {
        public async Task ConnectAsync() => await Task.Yield();

        public async Task DisconnectAsync() => await Task.Yield();

        public async Task SessionCloseAsync() => await Task.Yield();
    }
}
