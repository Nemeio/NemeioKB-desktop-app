using System;
using System.Windows.Input;
using Nemeio.Core.DataModels.Locale;

namespace Nemeio.Wpf.ViewModel
{
    public abstract class BaseActionViewModel : BaseViewModel
    {
        private readonly StringId _title;
        private ICommand _commandApply;

        public Action ClickAction { get; }

        public string Title
        {
            get
            {
                return _languageManager.GetLocalizedValue(_title);
            }
        }

        public string Tooltip
        {
            get
            {
                return Title;
            }
        }

        public BaseActionViewModel(StringId title, Action clickAction)
        {
            _title = title;
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
            return true;
        }

        private void Apply()
        {
            // do the actionc
            ClickAction();
        }
    }
}
