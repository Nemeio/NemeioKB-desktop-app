using System;
using System.Windows.Input;
using Nemeio.Presentation.Menu.Quit;

namespace Nemeio.Wpf.ViewModel
{
    public class QuitButtonViewModel : BaseViewModel
    {
        private QuitSection _section;
        private ICommand _commandApply;

        public QuitSection Section
        {
            get => _section;
            set
            {
                _section = value;
                NotifyPropertyChanged(nameof(Title));
            }
        }
        public string Title => _section?.Title ?? string.Empty;
        public Action ClickAction { get; set; }
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
            return ClickAction != null;
        }

        private void Apply()
        {
            // do the actionc
            ClickAction?.Invoke();
        }
    }
}
