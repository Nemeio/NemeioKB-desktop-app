using System.Windows;
using System.Windows.Input;
using Nemeio.Wpf.ViewModel;

namespace Nemeio.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        public UpdateWindow()
        {
            InitializeComponent();
            if (DataContext is UpdateViewModel viewModel)
            {
                viewModel.OnRequestClose += ViewModel_OnRequestClose;
            }
        }

        private void ViewModel_OnRequestClose(object sender, System.EventArgs e)
        {
            if (DataContext is UpdateViewModel viewModel)
            {
                viewModel.OnRequestClose -= ViewModel_OnRequestClose;
            }

            Close();
        }

        #region Events

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        #endregion
    }
}
