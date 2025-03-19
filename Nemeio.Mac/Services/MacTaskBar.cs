using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Icon;
using Nemeio.Core.Managers;
using Nemeio.Core.Services.Layouts;
using Nemeio.Mac.Services;
using Nemeio.Mac.Windows.Menu;
using Nemeio.Presentation;
using Nemeio.Presentation.Menu;

namespace Nemeio.Mac.StatusMenu
{
    public interface IMenuActionHandler
    {
        void ProceedUpdateActionHandler();

        void LayoutSelectedActionHandler(ILayout layout);
    }

    public class MacTaskBar : IMainUserInterface
    {
        #region Variables

        private IUIMenu _uiMenu;
        private MacTaskBarController _taskBarController;

        #endregion

        public MenuController Menu => _taskBarController.MenuController;

        #region Constructor

        public MacTaskBar(ILoggerFactory loggerFactory, IUIMenu uiMenu, IApplicationIconProvider iconProvider, ILanguageManager languageManager)
        {
            _taskBarController = new MacTaskBarController(loggerFactory, uiMenu, iconProvider, languageManager);

            _uiMenu = uiMenu ?? throw new ArgumentNullException(nameof(uiMenu));
            _uiMenu.Icon.OnValueChanged += Icon_OnValueChanged;
            _uiMenu.Menu.OnValueChanged += Menu_OnValueChanged;
            _uiMenu.Run();
        }

        private void Icon_OnValueChanged(object sender, Presentation.Menu.Tools.ObservableValueChangedEventArgs<Presentation.Menu.Icon.ApplicationIcon> e)
        {
            Debug.WriteLine("Icon_OnValueChanged");
            _taskBarController?.RefreshIcon();
        }


        private void Menu_OnValueChanged(object sender, Presentation.Menu.Tools.ObservableValueChangedEventArgs<Presentation.Menu.Menu> e)
        {
            _taskBarController?.MenuController?.RefreshMenu();
        }

        #endregion
    }
}
