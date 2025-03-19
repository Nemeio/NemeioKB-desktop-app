using System;
using System.Windows.Input;

namespace Nemeio.Wpf.ViewModel
{
    public class DebugButtonViewModel : BaseViewModel
    {
        private bool _enabled = false;
        private ICommand _commandApply;

        public Action ClickAction { get; }

        public string Title
        {
            get
            {
                return "DEBUG: Resync Layouts";
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                NotifyPropertyChanged(ref _enabled, value, "Enabled");
            }
        }

        public DebugButtonViewModel(Action clickAction)
        {
            ClickAction = clickAction;
        }

        public ICommand CommandApply
        {
            get
            {
                if (_commandApply == null)
                {
                    _commandApply = new RelayCommand(
                        param => this.Apply(),
                        param => this.CanApply()
                    );
                }
                return _commandApply;
            }
        }

        private bool CanApply()
        {
            // Verify command can be executed here
            return _enabled;
        }

        private void Apply()
        {
            // do the actionc
            ClickAction();
        }
    }
}
