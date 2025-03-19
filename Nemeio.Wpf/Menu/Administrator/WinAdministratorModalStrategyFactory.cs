using System;
using Nemeio.Core.Applications;
using Nemeio.Presentation.Menu.Administrator;
using Nemeio.Presentation.Modals;

namespace Nemeio.Wpf.Menu.Administrator
{
    public sealed class WinAdministratorModalStrategyFactory : IAdministratorModalStrategyFactory
    {
        private readonly IApplicationSettingsProvider _applicationSettingsManager;
        private readonly IModalWindowFactory _modalWindowFactory;

        public WinAdministratorModalStrategyFactory(IApplicationSettingsProvider applicationSettingsManager, IModalWindowFactory modalWindowFactory)
        {
            _applicationSettingsManager = applicationSettingsManager ?? throw new ArgumentNullException(nameof(applicationSettingsManager));
            _modalWindowFactory = modalWindowFactory ?? throw new ArgumentNullException(nameof(modalWindowFactory));
        }

        public IAdministratorModalStrategy Create()
        {
            var modalStrategy = new WinAdministratorModalStrategy(_applicationSettingsManager, _modalWindowFactory);

            return modalStrategy;
        }
    }
}
