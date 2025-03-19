using System.Threading.Tasks;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Tools.Stoppable;

namespace Nemeio.Core.Keyboard.Communication.Commands
{
    public interface IKeyboardCommandExecutor : IAsyncStoppable
    {
        Task Initialize();
        bool ScheduleCommand(IKeyboardCommand command);
        void RegisterNotification(CommandId commandId, Monitor monitor);
    }
}
