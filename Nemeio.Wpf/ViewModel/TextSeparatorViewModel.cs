using System;
using System.Windows;
using Nemeio.Core.DataModels.Locale;

namespace Nemeio.Wpf.ViewModel
{
    public class TextSeparatorViewModel : BaseViewModel
    {
        private readonly StringId _title;
        private readonly Func<bool> _shallDisplay;
        private bool _enabled = false;

        public string Title
        {
            get
            {
                return _languageManager.GetLocalizedValue(_title);
            }
        }

        public Visibility Visibility
        {
            get
            {
                return _shallDisplay() ? Visibility.Visible : Visibility.Collapsed;
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
                NotifyPropertyChanged(ref _enabled, value, nameof(Enabled));
            }
        }

        public TextSeparatorViewModel(StringId title, Func<bool> shallDisplay)
        {
            _title = title;
            _shallDisplay = shallDisplay;
        }

        public void CheckVisibility()
        {
            NotifyPropertyChanged("Visibility");
        }
    }
}
