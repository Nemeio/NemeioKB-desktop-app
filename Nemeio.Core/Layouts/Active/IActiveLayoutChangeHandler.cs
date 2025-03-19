using System.Threading.Tasks;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems.Applications;
using Nemeio.Core.Systems.Sessions;
using Nemeio.Core.Tools.Stoppable;

namespace Nemeio.Core.Layouts.Active
{
    public interface IActiveLayoutChangeHandler : IAsyncStoppable
    {
        IActiveLayoutSynchronizer Synchronizer { get; }
        Task RequestApplicationShutdownAsync(INemeio nemeio);
        Task RequestForegroundApplicationChangeAsync(INemeio nemeio, Application application);
        Task RequestHidSystemChangeAsync(INemeio nemeio);
        Task RequestHistoricChangeAsync(INemeio nemeio, bool isBack);
        Task RequestKeyboardSelectionChangeAsync(INemeio nemeio);
        Task RequestKeyPressChangeAsync(INemeio nemeio, LayoutId id);
        Task RequestMenuChangeAsync(INemeio nemeio, LayoutId id);
        Task RequestSessionChangeAsync(INemeio nemeio, SessionState state);
        Task RequestResetHistoricAsync();
    }
}
