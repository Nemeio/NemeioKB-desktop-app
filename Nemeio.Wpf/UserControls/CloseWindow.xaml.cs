using System;
using System.Windows;
using System.Windows.Input;

namespace Nemeio.Wpf.UserControls
{
    /// <summary>
    /// Interaction logic for CloseWindow.xaml
    /// </summary>
    public partial class CloseWindow : Window
    {
        static private CloseWindow _instance = null;

        static public bool Displayed
        {
            get
            {
                return _instance != null;
            }
        }

        static public CloseWindow Instance
        {
            get
            {
                return _instance;
            }
        }

        public CloseWindow()
        {
            InitializeComponent();
            if (Displayed)
            {
                throw new InvalidOperationException("The CloseWindow cannot be displayed twice");
            }
        }

        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
        private void CancelCloseApplication(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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

        private void StackPanel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
