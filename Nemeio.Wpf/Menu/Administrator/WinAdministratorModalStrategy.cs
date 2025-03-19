using System;
using Nemeio.Core.Applications;
using Nemeio.Core.Systems.Applications;
using Nemeio.Presentation.Menu.Administrator;
using Nemeio.Presentation.Modals;

namespace Nemeio.Wpf.Menu.Administrator
{
    public sealed class WinAdministratorModalStrategy : IAdministratorModalStrategy
    {
        private readonly IApplicationSettingsProvider _applicationSettingsManager;
        private readonly IModalWindowFactory _modalWindowFactory;

        private IModalWindow _administratorModal;

        public WinAdministratorModalStrategy(IApplicationSettingsProvider applicationSettingsManager, IModalWindowFactory modalWindowFactory)
        {
            _applicationSettingsManager = applicationSettingsManager ?? throw new ArgumentNullException(nameof(applicationSettingsManager));
            _modalWindowFactory = modalWindowFactory ?? throw new ArgumentNullException(nameof(modalWindowFactory));
        }

        public void OnForegroundApplicationChanged(Application application)
        {
            if (application != null && application.IsAdministrator && _applicationSettingsManager.ShowGrantPrivilegeWindow)
            {
                //  We accept only one administrator modal at once
                //  If a new one appear, this mean user change application

                if (_administratorModal != null)
                {
                    _administratorModal.Close();
                }

                _administratorModal = _modalWindowFactory.CreateAdministratorModal(application);
                _administratorModal.Display();
            }
        }
    }
}
