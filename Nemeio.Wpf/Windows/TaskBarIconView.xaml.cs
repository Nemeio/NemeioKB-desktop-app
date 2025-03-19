using System.Windows;
using Nemeio.Wpf.UserControls;
using Nemeio.Wpf.ViewModel;
using Nemeio.Wpf.Windows;

namespace Nemeio.Wpf
{
    /// <summary>
    /// Interaction logic for TaskBarIconView.xaml
    /// </summary>
    public partial class TaskBarIconView : Window
    {
        public TaskBarIconView()
        {
            InitializeComponent();
            if (App.StopRequired)
            {
                return;
            }
            TaskBarIconInstance.PreviewTrayPopupOpen += TaskBarIconInstance_PreviewTrayPopupOpen;

            if (TaskBarIconInstance.DataContext is TaskBarIconMenuViewModel viewModel)
            {
                viewModel.TrayIconStateChanged += ViewModel_TrayIconStateChanged;
            }
        }

        private void ViewModel_TrayIconStateChanged(object sender, TrayIconStateEventArgs e)
        {
            TaskBarIconInstance.TrayPopupResolved.IsOpen = e.IsOpen;
        }

        private void TaskBarIconInstance_PreviewTrayPopupOpen(object sender, RoutedEventArgs e)
        {
            // if close dialog is open, shunt the popup activation and bring close window in front
            if (CloseWindow.Displayed)
            {
                CloseWindow.Instance.Activate();
                e.Handled = true;
            }
            else if (DialogWindow.Displayed)
            {
                DialogWindow.Instance.Activate();
                e.Handled = true;
            }
            else if (LanguageSelectionWindow.Displayed)
            {
                LanguageSelectionWindow.Instance.Activate();
                e.Handled = true;
            }
            else if (FactoryResetModal.Displayed)
            {
                FactoryResetModal.Instance.Activate();
                e.Handled = true;
            }
        }
    }
}
