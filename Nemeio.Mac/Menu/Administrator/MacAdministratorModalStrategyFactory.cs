using System;
using Nemeio.Presentation.Menu.Administrator;

namespace Nemeio.Mac.Menu.Administrator
{
    public sealed class MacAdministratorModalStrategyFactory : IAdministratorModalStrategyFactory
    {
        public IAdministratorModalStrategy Create()
        {
            return new MacAdministratorModalStrategy();
        }
    }
}
