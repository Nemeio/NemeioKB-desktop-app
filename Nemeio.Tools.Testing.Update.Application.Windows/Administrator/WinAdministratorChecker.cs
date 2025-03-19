using System.Security.Principal;
using Nemeio.Tools.Testing.Update.Core.Administrator;

namespace Nemeio.Tools.Testing.Update.Application.Windows.Administrator
{
    public class WinAdministratorChecker : IAdministratorChecker
    {
        public bool RunAsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
