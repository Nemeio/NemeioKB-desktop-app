using System.Windows;
using System.Windows.Controls;

namespace Nemeio.Wpf.UserControls
{
    /// <summary>
    /// Interaction logic for DebugButtonViewModel.xaml
    /// </summary>
    public partial class DebugButtonView : UserControl
    {
        public DebugButtonView()
        {
            InitializeComponent();
#if DEBUG
            Visibility = Visibility.Visible;
#endif

        }
    }
}
