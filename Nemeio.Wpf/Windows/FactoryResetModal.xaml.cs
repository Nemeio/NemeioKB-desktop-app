using System;
using System.Windows;
using System.Windows.Input;
using Nemeio.Wpf.ViewModel;

namespace Nemeio.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for FactoryResetModal.xaml
    /// </summary>
    public partial class FactoryResetModal : Window
    {
        static private FactoryResetModal _instance = null;

        #region Properties

        static public bool Displayed
        {
            get
            {
                return _instance != null;
            }
        }

        static public FactoryResetModal Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        public FactoryResetModal()
        {
            InitializeComponent();
            if (Displayed)
            {
                throw new InvalidOperationException("This modal cannot be displayed twice");
            }
            if (DataContext is FactoryResetViewModel viewModel)
            {
                viewModel.OnRequestClose += ViewModel_OnRequestClose;
            }
        }

        #region Events

        private void ViewModel_OnRequestClose(object sender, System.EventArgs e)
        {
            if (DataContext is FactoryResetViewModel viewModel)
            {
                viewModel.OnRequestClose -= ViewModel_OnRequestClose;
            }

            Close();
        }

        private void Window_Activated(object sender, System.EventArgs e)
        {
            _instance = this;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            _instance = null;
        }

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
