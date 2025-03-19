using System.Windows;
using Nemeio.Presentation.Menu.Synchronization;

namespace Nemeio.Wpf.ViewModel
{
    public class SynchronizationViewModel : BaseViewModel
    {
        private SynchronizationSection _section;

        public SynchronizationSection Section
        {
            get => _section;
            set
            {
                _section = value;

                NotifyPropertyChanged(nameof(Title));
                NotifyPropertyChanged(nameof(Progress));
                NotifyPropertyChanged(nameof(Visibility));
            }
        }

        public string Title => _section?.Title ?? string.Empty;
        public string Progress => _section?.ProgressDescription ?? string.Empty;
        public Visibility Visibility => _section?.Visible ?? false ? Visibility.Visible : Visibility.Collapsed;
    }
}
