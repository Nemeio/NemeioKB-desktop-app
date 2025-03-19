using SystemApplication = Nemeio.Core.Systems.Applications.Application;

namespace Nemeio.Presentation.Menu.Administrator
{
    public interface IAdministratorModalStrategy
    {
        void OnForegroundApplicationChanged(SystemApplication application);
    }
}
