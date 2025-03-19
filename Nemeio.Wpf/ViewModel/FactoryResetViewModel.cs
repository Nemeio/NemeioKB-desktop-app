using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using MvvmCross.Platform;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.FactoryReset;
using Nemeio.Core.Keyboard.Nemeios.Proxy;
using Nemeio.Wpf.Models;

namespace Nemeio.Wpf.ViewModel
{
    public class FactoryResetViewModel : BaseViewModel
    {
        private class FactoryResetNemeioProxy : KeyboardProxy, IFactoryResetHolder
        {
            private readonly IFactoryResetHolder _factoryResetHolder;

            public FactoryResetNemeioProxy(Core.Keyboard.Nemeio nemeio)
                : base(nemeio)
            {
                _factoryResetHolder = nemeio as IFactoryResetHolder;
            }

            public Task CancelFactoryResetAsync() => _factoryResetHolder.CancelFactoryResetAsync();

            public Task ConfirmFactoryResetAsync() => _factoryResetHolder.ConfirmFactoryResetAsync();

            public Task WantFactoryResetAsync() => _factoryResetHolder.WantFactoryResetAsync();
        }

        #region Variables

        private readonly ILogger _logger;
        private readonly IKeyboardController _keyboardController;

        private string _title;
        private string _explanation;
        private string _validButtonText;
        private ICommand _validCommand;

        #endregion

        #region Properties

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyPropertyChanged(nameof(Title));
            }
        }

        public string Explanation
        {
            get => _explanation;
            set
            {
                _explanation = value;
                NotifyPropertyChanged(nameof(Explanation));
            }
        }

        
        public string ValidButtonText
        {
            get => _validButtonText;
            set
            {
                _validButtonText = value;
                NotifyPropertyChanged(nameof(ValidButtonText));
            }
        }

        public ICommand ValidCommand
        {
            get
            {
                if (_validCommand == null)
                {
                    _validCommand = new CommandHandler(async delegate
                    {
                        RequestClose();
                        StartFactoryReset();
                    }, () => true);
                }

                return _validCommand;
            }
        }

        #endregion

        #region Constructor

        public FactoryResetViewModel()
        {
            _logger = Mvx.Resolve<ILoggerFactory>().CreateLogger<FactoryResetViewModel>();
            _keyboardController = Mvx.Resolve<IKeyboardController>();

            Title = _languageManager.GetLocalizedValue(StringId.FactoryResetTitle);
            Explanation = _languageManager.GetLocalizedValue(StringId.FactoryResetExplanation);
            ValidButtonText = _languageManager.GetLocalizedValue(StringId.FactoryResetValidButtonText);
        }

        #endregion

        private void StartFactoryReset()
        {
            if (_keyboardController.Connected)
            {
                var proxy = KeyboardProxy.CastTo<FactoryResetNemeioProxy>(_keyboardController.Nemeio);
                if (proxy != null)
                {
                    try
                    {
                        Task.Run(() => proxy.ConfirmFactoryResetAsync());
                    }
                    catch (FactoryResetFailedException exception)
                    {
                        _logger.LogError(exception, $"ConfirmFactoryResetAsync failed");
                    }
                }
            }
        }
    }
}
