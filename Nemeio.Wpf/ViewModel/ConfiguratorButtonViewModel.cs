using System;
using System.Windows;
using System.Windows.Input;
using MvvmCross.Platform;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Managers;
using Nemeio.Core.Services;
using Nemeio.Presentation.Menu.Configurator;
using Nemeio.Wpf.Windows;

namespace Nemeio.Wpf.ViewModel
{
    public class ConfiguratorButtonViewModel : BaseViewModel
    {
        private ConfiguratorSection _section;
        private ICommand _commandApply;

        public ConfiguratorSection Section
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

        public ConfiguratorButtonViewModel() { }
    }
}
