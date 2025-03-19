using System;
using System.Windows.Input;
using System.Windows.Threading;
using Nemeio.Core.PackageUpdater;
using Nemeio.Presentation.Menu.Version;

namespace Nemeio.Wpf.ViewModel
{
    public class VersionViewModel : BaseViewModel
    {
        private readonly Dispatcher _dispatcher;

        private VersionSection _section;
        private ICommand _commandApply;

        public Action ClickAction { get; set; }
        public string Title => _section?.Title ?? string.Empty;
        public string ApplicationVersion => _section?.ApplicationVersionTitle ?? string.Empty;
        public bool KeyboardIsPlugged => _section?.KeyboardIsPlugged ?? false;
        public string KeyboardStmVersion => _section?.Stm32VersionTitle ?? string.Empty;
        public string KeyboardNrfVersion => _section?.BluetoothLEVersionTitle ?? string.Empty;
        public string KeyboardIteVersion => _section?.IteVersionTitle ?? string.Empty;
        public string Update => _section?.UpdateStatus ?? string.Empty;
        public ICommand CommandApply
        {
            get
            {
                if (_commandApply == null)
                {
                    _commandApply = new RelayCommand(
                        param => Apply(),
                        param => CanApply()
                    );
                }
                return _commandApply;
            }
        }
        public PackageUpdateState UpdateKind => _section?.UpdateKind ?? PackageUpdateState.Idle;

        public VersionSection Section
        {
            get => _section;
            set
            {
                _section = value;
                NotifyPropertyChanged(nameof(Title));
                NotifyPropertyChanged(nameof(ApplicationVersion));
                NotifyPropertyChanged(nameof(KeyboardIsPlugged));
                NotifyPropertyChanged(nameof(KeyboardStmVersion));
                NotifyPropertyChanged(nameof(KeyboardNrfVersion));
                NotifyPropertyChanged(nameof(KeyboardIteVersion));
                NotifyPropertyChanged(nameof(Update));
                NotifyPropertyChanged(nameof(UpdateKind));
            }
        }

        public VersionViewModel() 
            : base()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        private bool CanApply()
        {
            // Verify command can be executed here
            return ClickAction != null;
        }

        private void Apply()
        {
            // do the actionc
            ClickAction?.Invoke();
        }
    }
}
