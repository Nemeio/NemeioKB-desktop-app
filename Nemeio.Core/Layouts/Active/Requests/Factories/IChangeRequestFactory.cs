using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts.Active.Requests.Base;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems.Applications;
using Nemeio.Core.Systems.Sessions;

namespace Nemeio.Core.Layouts.Active.Requests.Factories
{
    public interface IChangeRequestFactory
    {
        IChangeRequest CreateApplicationShutdownRequest(INemeio nemeio);
        IChangeRequest CreateForegroundApplicationRequest(INemeio nemeio, Application application);
        IChangeRequest CreateHidSystemRequest(INemeio nemeio);
        IChangeRequest CreateHistoricRequest(INemeio nemeio, bool isBack);
        IChangeRequest CreateKeyboardSelectionRequest(INemeio nemeio);
        IChangeRequest CreateKeyPressRequest(INemeio nemeio, ILayout layout);
        IChangeRequest CreateKeyPressRequest(INemeio nemeio, LayoutId id);
        IChangeRequest CreateMenuRequest(INemeio nemeio, ILayout layout);
        IChangeRequest CreateMenuRequest(INemeio nemeio, LayoutId id);
        IChangeRequest CreateSessionRequest(INemeio nemeio, SessionState state);
    }
}
