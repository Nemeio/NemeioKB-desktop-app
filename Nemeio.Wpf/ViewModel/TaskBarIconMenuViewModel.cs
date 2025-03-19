using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Icon;
using Nemeio.Core.Services.Layouts;
using Nemeio.Presentation;
using Nemeio.Presentation.Menu;
using Nemeio.Presentation.Menu.Layouts;

namespace Nemeio.Wpf.ViewModel
{
    public class TrayIconStateEventArgs : EventArgs
    {
        public bool IsOpen { get; private set; }

        public TrayIconStateEventArgs(bool isOpen)
        {
            IsOpen = isOpen;
        }
    }

    public class TaskBarIconMenuViewModel : IMainUserInterface, INotifyPropertyChanged
    {
        private readonly Dispatcher _dispatcher;
        private readonly ILogger _logger;
        private readonly IUIMenu _uiMenu;
        private readonly IApplicationIconProvider _iconProvider;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties

        public QuitButtonViewModel QuitButton { get; }
        public ConnectionViewModel ConnectionInfo { get; }
        public BatteryViewModel BatteryInfo { get; }
        public VersionViewModel VersionInfo { get; }
        public SynchronizationViewModel SynchronizationInfo { get; }
        public ConfiguratorButtonViewModel ConfiguratorButton { get; }
        public TextSeparatorViewModel StandardHeader { get; }
        public TextSeparatorViewModel CustomHeader { get; }
        public TextSeparatorViewModel EmptyHeader { get; }
        public ObservableCollection<LayoutViewModel> StandardList { get; private set; }
        public ObservableCollection<LayoutViewModel> CustomList { get; private set; }
        public string IconSourcePath { get; private set; }
        public double LayoutListMaxHeight
        {
            get
            {
                return SystemParameters.PrimaryScreenHeight / 3;
            }
        }

        public event EventHandler<TrayIconStateEventArgs> TrayIconStateChanged;

        #endregion

        public TaskBarIconMenuViewModel(ILoggerFactory loggerFactory, IUIMenu uiMenu, IApplicationIconProvider iconProvider)
        {
            QuitButton = new QuitButtonViewModel();
            QuitButton.ClickAction = () => _uiMenu.DisplayQuitModal();
            ConnectionInfo = new ConnectionViewModel();
            BatteryInfo = new BatteryViewModel();
            StandardHeader = new TextSeparatorViewModel(StringId.CommonStandard, ShowStandardHeader);
            CustomHeader = new TextSeparatorViewModel(StringId.CommonCustom, ShowCustomHeader);
            EmptyHeader = new TextSeparatorViewModel(StringId.Empty, () => { return true; }) { Enabled = true };
            VersionInfo = new VersionViewModel();
            VersionInfo.ClickAction = () => _uiMenu.DisplayUpdateModalIfNeeded();
            SynchronizationInfo = new SynchronizationViewModel();
            ConfiguratorButton = new ConfiguratorButtonViewModel();
            ConfiguratorButton.ClickAction = () => _uiMenu.DisplayConfiguratorModal();
            StandardList = new ObservableCollection<LayoutViewModel>();
            CustomList = new ObservableCollection<LayoutViewModel>();

            _dispatcher = Dispatcher.CurrentDispatcher;
            _logger = loggerFactory.CreateLogger<TaskBarIconMenuViewModel>();
            _iconProvider = iconProvider ?? throw new ArgumentNullException(nameof(iconProvider));

            _uiMenu = uiMenu ?? throw new ArgumentNullException(nameof(uiMenu));
            _uiMenu.Icon.OnValueChanged += Icon_OnValueChanged;
            _uiMenu.State.OnValueChanged += MenuState_OnValueChanged;
            _uiMenu.Menu.OnValueChanged += Menu_OnValueChanged;
            _uiMenu.Run();
        }

        private void Menu_OnValueChanged(object sender, Presentation.Menu.Tools.ObservableValueChangedEventArgs<Presentation.Menu.Menu> e)
        {
            _dispatcher.Invoke(() =>
            {
                var menu = e.Value;

                RefreshBattery(menu);
                RefreshConnection(menu);
                RefreshVersion(menu);
                RefreshLayouts(menu);
                RefreshSynchronization(menu);
                RefreshQuit(menu);
                RefreshConfigurator(menu);
            });
        }

        #region Synchronization

        private void RefreshSynchronization(Presentation.Menu.Menu menu)
        {
            SynchronizationInfo.Section = menu.Synchronization;
        }

        #endregion

        #region Layouts

        public Task SetLayoutSelectionAsync(ILayout layout) => _uiMenu.SelectLayoutAsync(layout);

        private void RefreshLayouts(Presentation.Menu.Menu menu)
        {
            var layoutSection = menu.Layouts;
            if (layoutSection.Visible)
            {
                StandardList = ConstructLayoutViewModels(layoutSection.Subsections, true);
                CustomList = ConstructLayoutViewModels(layoutSection.Subsections, false);
            }
            else
            {
                var emptyList = Enumerable.Empty<LayoutViewModel>();
                var emptyObservableList = new ObservableCollection<LayoutViewModel>(emptyList);

                StandardList = emptyObservableList;
                CustomList = emptyObservableList;
            }

            StandardHeader.CheckVisibility();
            CustomHeader.CheckVisibility();

            NotifyPropertyChanged(nameof(StandardList));
            NotifyPropertyChanged(nameof(CustomList));
        }

        private bool ShowStandardHeader() => StandardList.Count > 0 && ConnectionInfo.Visibility == Visibility.Visible;

        private bool ShowCustomHeader() => CustomList.Count > 0;

        private ObservableCollection<LayoutViewModel> ConstructLayoutViewModels(IList<LayoutSubsection> subsections, bool standard)
        {

            var viewModels = subsections
                .Where(x => x.IsStandard == standard)
                .Select(x => new LayoutViewModel() { Subsection = x })
                .ToList().OrderBy(x => x.Layout.Order);



            return new ObservableCollection<LayoutViewModel>(viewModels);
        }

        #endregion

        #region Versions

        private void RefreshVersion(Presentation.Menu.Menu menu)
        {
            _dispatcher.Invoke(() =>
            {
                VersionInfo.Section = menu.Versions;
            });
        }

        #endregion

        #region Connection

        private void RefreshConnection(Presentation.Menu.Menu menu)
        {
            ConnectionInfo.Section = menu.Connection;
        }

        #endregion

        #region Icon

        private void Icon_OnValueChanged(object sender, Presentation.Menu.Tools.ObservableValueChangedEventArgs<Presentation.Menu.Icon.ApplicationIcon> e)
        {
            RefreshIcon();
        }

        private void RefreshIcon()
        {
            _dispatcher.Invoke(() =>
            {
                IconSourcePath = _iconProvider.GetIconResourceFromCurrentState();
                NotifyPropertyChanged(nameof(IconSourcePath));
            });
        }

        #endregion

        #region Battery

        private void RefreshBattery(Presentation.Menu.Menu menu)
        {
            BatteryInfo.Value = menu.Battery;
        }

        #endregion

        #region Quit

        private void RefreshQuit(Presentation.Menu.Menu menu)
        {
            QuitButton.Section = menu.Quit;
        }

        #endregion

        #region Configurator

        private void RefreshConfigurator(Presentation.Menu.Menu menu)
        {
            ConfiguratorButton.Section = menu.Configurator;
        }

        #endregion

        #region Menu State

        private void MenuState_OnValueChanged(object sender, Presentation.Menu.Tools.ObservableValueChangedEventArgs<MenuState> e) => TrayIconStateChanged?.Invoke(this, new TrayIconStateEventArgs(e.Value == MenuState.Open));

        #endregion

        #region MVVM

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Template for property change event notification using this as a base template to prevent property misspelling
        /// (property identified from variable name)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="aVariable">Variable to be changed</param>
        /// <param name="aValue">Target to be set to variable</param>
        /// <param name="aPropertyName"></param>
        /// <returns></returns>
        protected bool NotifyPropertyChanged<T>(ref T aVariable, T aValue, [CallerMemberName] string aPropertyName = null)
        {
            if (object.Equals(aVariable, aValue))
            {
                return false;
            }

            aVariable = aValue;
            NotifyPropertyChanged(aPropertyName);
            return true;
        }


        #endregion
    }
}
