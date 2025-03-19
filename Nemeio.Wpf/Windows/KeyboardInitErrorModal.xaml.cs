using System.Windows;
using System.Windows.Input;
using Nemeio.Wpf.ViewModel;

namespace Nemeio.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for KeyboardInitErrorModal.xaml
    /// </summary>
    public partial class KeyboardInitErrorModal : Window
    {
        public KeyboardInitErrorModal()
        {
            InitializeComponent();
            if (DataContext is KeyboardInitErrorModalViewModel viewModel)
            {
                viewModel.OnRequestClose += ViewModel_OnRequestClose;
            }
        }

        private void ViewModel_OnRequestClose(object sender, System.EventArgs e)
        {
            if (DataContext is KeyboardInitErrorModalViewModel viewModel)
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
