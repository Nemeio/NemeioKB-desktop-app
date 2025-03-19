using System;
using System.Windows;
using System.Windows.Input;

namespace Nemeio.Wpf.UserControls
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml to be modal
    /// </summary>
    public partial class DialogWindow : Window
    {
        static private DialogWindow _instance = null;

        static public bool Displayed
        {
            get
            {
                return _instance != null;
            }
        }

        static public DialogWindow Instance
        {
            get
            {
                return _instance;
            }
        }

        public bool Modal { get; }

        public DialogWindow(bool modal = true)
        {
            InitializeComponent();
            if (Displayed && modal)
            {
                throw new InvalidOperationException("The DialogWindow cannot be displayed twice when modal");
            }
            Modal = modal;
        }

        private void YesClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
        private void NoClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
            Close();
        }

        private void Window_Activated(object sender, System.EventArgs e)
        {
            if (Modal)
            {
                _instance = this;
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            if (Modal)
            {
                _instance = null;
            }
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
