using System;
using System.Windows;
using System.Windows.Input;
using Nemeio.Wpf.ViewModel;

namespace Nemeio.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for LanguageSelectionWindow.xaml
    /// </summary>
    public partial class LanguageSelectionWindow : Window
    {
        static private LanguageSelectionWindow _instance = null;

        static public bool Displayed
        {
            get
            {
                return _instance != null;
            }
        }

        static public LanguageSelectionWindow Instance
        {
            get
            {
                return _instance;
            }
        }

        public LanguageSelectionWindow()
        {
            InitializeComponent();
            if (Displayed)
            {
                throw new InvalidOperationException("This modal cannot be displayed twice");
            }
            if (DataContext is SelectionLanguageViewModel viewModel)
            {
                viewModel.OnRequestClose += ViewModel_OnRequestClose;
            }
        }

        private void ViewModel_OnRequestClose(object sender, EventArgs e)
        {
            if (DataContext is SelectionLanguageViewModel viewModel)
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

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
