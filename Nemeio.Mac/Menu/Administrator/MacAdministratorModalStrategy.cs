using Nemeio.Core.Systems.Applications;
using Nemeio.Presentation.Menu.Administrator;

namespace Nemeio.Mac.Menu.Administrator
{
    public class MacAdministratorModalStrategy : IAdministratorModalStrategy
    {
        public void OnForegroundApplicationChanged(Core.Systems.Applications.Application application)
        {
            //  Nothing to do here
            //  Mac application doesn't manage administrator mode
        }
    }
}
