using System.Threading.Tasks;
using Nemeio.Core.Services.Layouts;
using Nemeio.Presentation.Menu.Icon;
using Nemeio.Presentation.Menu.Tools;

namespace Nemeio.Presentation.Menu
{
    public interface IUIMenu
    {
        
        ObservableValue<ApplicationIcon> Icon { get; }
        ObservableValue<MenuState> State { get; }
        ObservableValue<Menu> Menu { get; }

        void Run();
        void DisplayUpdateModalIfNeeded();
        void DisplayQuitModal();
        void DisplayConfiguratorModal();
        Task SelectLayoutAsync(ILayout layout);
        void OpenMenu();
        void CloseMenu();
    }
}
