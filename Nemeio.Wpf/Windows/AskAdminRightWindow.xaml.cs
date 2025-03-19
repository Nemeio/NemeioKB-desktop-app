using System.Windows;
using System.Windows.Input;
using Nemeio.Wpf.ViewModel;

namespace Nemeio.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for AskAdminRight.xaml
    /// </summary>
    public partial class AskAdminRightWindow : Window
    {
        public AskAdminRightWindow(string processName)
        {
            InitializeComponent();

            if (DataContext is AskAdminRightViewModel viewModel)
            {
                viewModel.ProcessName = processName;
                viewModel.OnRequestClose += ViewModel_OnRequestClose;
            }
        }

        private void ViewModel_OnRequestClose(object sender, System.EventArgs e)
        {
            if (DataContext is AskAdminRightViewModel viewModel)
            {
                viewModel.OnRequestClose -= ViewModel_OnRequestClose;
            }

            Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
