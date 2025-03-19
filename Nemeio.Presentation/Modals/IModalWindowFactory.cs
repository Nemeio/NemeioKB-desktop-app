using SystemApplication = Nemeio.Core.Systems.Applications.Application;

namespace Nemeio.Presentation.Modals
{
    public interface IModalWindowFactory
    {
        IModalWindow CreateUpdateModal();
        IModalWindow CreateQuitModal();
        IModalWindow CreateLanguageSelectionModal();
        IModalWindow CreateConfiguratorModal();
        IModalWindow CreateFactoryResetModal();
        IModalWindow CreateKeyboardInitErrorModal();
        IModalWindow CreateAdministratorModal(SystemApplication application);
        IModalWindow CreateRemovedByHidModal();
    }
}
