using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using MvvmCross.Platform;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.PackageUpdater.Updatable;
using Nemeio.Core.PackageUpdater.Updatable.States;
using Nemeio.Core.Tools.StateMachine;
using Nemeio.Presentation.PackageUpdater.UI;
using Nemeio.Wpf.Models;

namespace Nemeio.Wpf.ViewModel
{
    public enum UpdateViewType
    {
        Actions,
        Download,
        Installing
    }

    public class UpdateViewModel : BaseViewModel
    {
        #region Variables

        private readonly Dispatcher _dispatcher;
        private readonly IPackageUpdater _packageUpdater;
        private readonly IPackageUpdaterMessageFactory _messageFactory;

        private string _title;
        private string _pageTitle;
        private string _pageSubtitle;
        private string _pageMainAction;
        private string _pageSecondaryAction;
        private double _downloadProgress;
        private double _installProgress;
        private string _installComponentName;

        private UpdateViewType _viewState;

        private ICommand _closeCommand;
        private ICommand _downloadCommand;
        private ICommand _installCommand;

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

        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                _pageTitle = value;
                NotifyPropertyChanged(nameof(PageTitle));
            }
        }

        public string PageSubtitle
        {
            get => _pageSubtitle;
            set
            {
                _pageSubtitle = value;
                NotifyPropertyChanged(nameof(PageSubtitle));
            }
        }

        public string PageMainAction
        {
            get => _pageMainAction;
            set
            {
                _pageMainAction = value;
                NotifyPropertyChanged(nameof(PageMainAction));
            }
        }

        public string PageSecondaryAction
        {
            get => _pageSecondaryAction;
            set
            {
                _pageSecondaryAction = value;
                NotifyPropertyChanged(nameof(PageSecondaryAction));
            }
        }

        public UpdateViewType ViewState
        {
            get => _viewState;
            set
            {
                _viewState = value;
                NotifyPropertyChanged(nameof(ViewState));
            }
        }

        public double DownloadProgress
        {
            get => _downloadProgress;
            set
            {
                _downloadProgress = value;
                NotifyPropertyChanged(nameof(DownloadProgress));
            }
        }

        public double InstallProgress
        {
            get => _installProgress;
            set
            {
                _installProgress = value;
                NotifyPropertyChanged(nameof(InstallProgress));
            }
        }

        public string InstallComponentName
        {
            get => _installComponentName;
            set
            {
                _installComponentName = value;
                NotifyPropertyChanged(nameof(InstallComponentName));
            }
        }

        public ICommand MainActionCommand
        {
            get
            {
                //  FIXME [KSB] : Maybe there are a better way to do
                if (_packageUpdater.Component is UpdatableSoftware)
                {
                    if (_packageUpdater.State == PackageUpdateState.DownloadPending)
                    {
                        if (_downloadCommand == null)
                        {
                            _downloadCommand = new CommandHandler(() =>
                            {
                                ViewState = UpdateViewType.Download;
                                Task.Run(async () => await _packageUpdater.DownloadPendingUpdateAsync());
                            }, () => true);
                        }

                        return _downloadCommand;
                    }
                    else if (_packageUpdater.State == PackageUpdateState.UpdatePending)
                    {
                        return BuildInstallCommandIfNeeded();
                    }
                    else
                    {
                        return new CommandHandler(() => { }, () => false);
                    }
                }
                else
                {
                    if (_packageUpdater.State == PackageUpdateState.UpdatePending)
                    {
                        return BuildInstallCommandIfNeeded();
                    }
                    else
                    {
                        return new CommandHandler(() => { }, () => false);
                    }
                }
            }
        }

        private ICommand BuildInstallCommandIfNeeded()
        {
            if (_installCommand == null)
            {
                _installCommand = new CommandHandler(async () =>
                {
                    await _packageUpdater.InstallUpdateAsync();
                }, () => true);
            }

            return _installCommand;
        }

        public ICommand SecondaryActionCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new CommandHandler(async () =>
                    {
                        RequestClose();

                        if (_packageUpdater.State == PackageUpdateState.UpdateFailed || 
                            _packageUpdater.State == PackageUpdateState.UpdateSucceed)
                        {
                            await _packageUpdater.CheckUpdatesAsync();
                        }
                    }, () => true);
                }

                return _closeCommand;
            }
        }

        #endregion

        #region Constructors

        public UpdateViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;

            _packageUpdater = Mvx.Resolve<IPackageUpdater>();
            _packageUpdater.OnStateChanged += PackageUpdater_StateChanged;
            _packageUpdater.OnUpdateAvailable += PackageUpdater_OnUpdateAvailable;

            _messageFactory = Mvx.Resolve<IPackageUpdaterMessageFactory>();

            Title = _languageManager.GetLocalizedValue(StringId.PackageUpdaterTitle);
            DownloadProgress = 0;

            RefreshPage();
            CheckUpdate();
        }

        private void CheckUpdate()
        {
            if (_packageUpdater.Component != null)
            {
                _packageUpdater.Component.OnUpdateStateChanged += Component_OnUpdateStateChanged;
                _packageUpdater.Component.OnUpdateFinished += Component_OnUpdateFinished;
            }
        }

        ~UpdateViewModel()
        {
            if (_packageUpdater != null)
            {
                _packageUpdater.OnUpdateAvailable -= PackageUpdater_OnUpdateAvailable;
                _packageUpdater.OnStateChanged -= PackageUpdater_StateChanged;
            }
        }

        #endregion

        public override void LanguageChanged()
        {
            base.LanguageChanged();
            DetermineText();
        }

        #region Events

        private void PackageUpdater_StateChanged(object sender, OnStateChangedEventArgs<PackageUpdateState> e)
        {
            _dispatcher.Invoke(() =>
            {
                try
                {
                    RefreshPage();
                }
                catch (InvalidOperationException)
                {
                    //  If an error occured we close current popup
                    RequestClose();
                }
            });
        }

        private void PackageUpdater_OnUpdateAvailable(object sender, EventArgs e)
        {
            CheckUpdate();
        }

        private void Component_OnUpdateFinished(object sender, UpdateFinishedEventArgs e)
        {
            if (_packageUpdater != null && _packageUpdater.Component != null)
            {
                _packageUpdater.Component.OnUpdateFinished -= Component_OnUpdateFinished;
                _packageUpdater.Component.OnUpdateStateChanged -= Component_OnUpdateStateChanged;
            }
        }

        private async void Component_OnUpdateStateChanged(object sender, UpdateStateChangedEventArgs e)
        {
            var state = e.State;
            
            if (state is UpdateInProgressState progressState)
            {
                await _dispatcher.InvokeAsync(() =>
                {
                    var message = _messageFactory.GetMessage(_packageUpdater.State);
                    var componentName = _languageManager.GetLocalizedValue(_messageFactory.GetNameForComponent(progressState.Progress.Name));
                    if (progressState.Progress.Name == Core.Keyboard.Updates.Progress.UpdateComponent.Unknown)
                    {
                        PageTitle = $"{message.Title}";
                    }
                    else
                    {
                        PageTitle = $"{message.Title} : {componentName}";
                    }
                    
                    InstallComponentName = progressState.Progress.Name.ToString();
                    InstallProgress = progressState.Progress.Percent;
                });
            }
            else if (state is UpdateDownloadingState downloadingState)
            {
                await _dispatcher.InvokeAsync(() => 
                {
                    var message = _messageFactory.GetMessage(_packageUpdater.State);

                    PageTitle = $"{message.Title} : {downloadingState.FileIndex + 1}/{downloadingState.FileCount}";
                    DownloadProgress = downloadingState.Percent;
                });
            }
        }

        #endregion

        private void DetermineText()
        {
            var message = _messageFactory.GetMessage(_packageUpdater.State);
            if (message != null)
            {
                PageTitle = message.Title;
                PageSubtitle = message.Subtitle;
                PageMainAction = message.MainAction;
                PageSecondaryAction = message.SecondaryAction;
            }
        }

        private UpdateViewType DetermineViewState()
        {
            switch (_packageUpdater.State)
            {
                case PackageUpdateState.WaitUsbKeyboard:
                case PackageUpdateState.UpdateFailed:
                case PackageUpdateState.UpdateSucceed:
                case PackageUpdateState.UpdatePending:
                case PackageUpdateState.DownloadPending:
                    return UpdateViewType.Actions;
                case PackageUpdateState.ApplyUpdate:
                    return UpdateViewType.Installing;
                case PackageUpdateState.Downloading:
                    return UpdateViewType.Download;
                case PackageUpdateState.Idle:
                default:
                    throw new InvalidOperationException("Not supported step");
            }
        }

        /// <summary>
        /// Refresh UI with new data
        /// Must be called on Main thread
        /// </summary>
        private void RefreshPage()
        {
            DetermineText();
            ViewState = DetermineViewState();
            NotifyPropertyChanged(nameof(MainActionCommand));
            NotifyPropertyChanged(nameof(SecondaryActionCommand));
        }
    }
}
