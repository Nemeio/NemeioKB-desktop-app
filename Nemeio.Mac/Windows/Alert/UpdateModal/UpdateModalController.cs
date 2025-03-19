using System;
using System.Threading.Tasks;
using AppKit;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Managers;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.PackageUpdater.Downloader;
using Nemeio.Core.PackageUpdater.Updatable;
using Nemeio.Core.PackageUpdater.Updatable.States;
using Nemeio.Core.Tools.StateMachine;
using Nemeio.Mac.Models;
using Nemeio.Presentation.PackageUpdater.UI;

namespace Nemeio.Mac.Windows.Alert.UpdateModal
{
    public partial class UpdateModalController : NSViewController, INSWindowDelegate
    {
        private nuint LeftButtonMask = 0x1;
        private nuint RightButtonMask = 0x2;
        private const string MainActionButtonSelector = "mainActionSelector";
        private const string SecondaryActionButtonSelector = "secondaryActionSelector";

        private const string ViewStoryboardName = "UpdateModal";
        private const string ViewControllerIdentifier = "UpdateModalController";

        public ILanguageManager LanguageManager { get; set; }
        public IPackageUpdater PackageUpdater { get; set; }
        public IPackageUpdaterMessageFactory MessageFactory { get; set; }
        public Action DidViewDismiss { get; set; }

        #region Constructors

        public static UpdateModalController Create(IPackageUpdater packageUpdater, ILanguageManager languageManager, IPackageUpdaterMessageFactory messageFactory, Action didViewDismiss)
        {
            var storyboard = NSStoryboard.FromName(ViewStoryboardName, null);
            var viewController = storyboard.InstantiateControllerWithIdentifier(ViewControllerIdentifier) as UpdateModalController;
            viewController.PackageUpdater = packageUpdater;
            viewController.LanguageManager = languageManager;
            viewController.MessageFactory = messageFactory;
            viewController.DidViewDismiss = didViewDismiss;

            return viewController;
        }

        // Called when created from unmanaged code
        public UpdateModalController(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public UpdateModalController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public UpdateModalController()
            : base("UpdateModal", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize() { }

        #endregion

        #region Lifecycle Methods

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CheckUpdate();
        }

        public override void ViewWillAppear()
        {
            base.ViewWillAppear();

            SetupView();
            RefreshUI();

            PreferredContentSize = new CGSize(480, 300);

            PackageUpdater.OnStateChanged += PackageUpdater_StepChanged;
            PackageUpdater.OnUpdateAvailable += PackageUpdater_OnUpdateAvailable;
        }

        public override void ViewDidAppear()
        {
            base.ViewDidAppear();

            View.Window.StyleMask = NSWindowStyle.Closable | NSWindowStyle.Titled;
            View.Window.Delegate = this;
        }

        public override void ViewDidDisappear()
        {
            base.ViewDidDisappear();

            PackageUpdater.OnStateChanged -= PackageUpdater_StepChanged;
        }

        #endregion

        private void SetupView()
        {
            View.Appearance = NSAppearance.GetAppearance(NSAppearance.NameDarkAqua);
            View.WantsLayer = true;
            View.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.DarkPurple).CGColor;

            TitleLabel.Editable = false;
            TitleLabel.TextColor = MacColorHelper.FromHex(NemeioColor.Green);

            TitleDivider.WantsLayer = true;
            TitleDivider.Layer.BackgroundColor = NSColor.White.CGColor;

            TitleIconContainer.WantsLayer = true;
            TitleIconContainer.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.Purple).CGColor;

            CrossImageContainer.WantsLayer = true;
            CrossImageContainer.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.Red).CGColor;

            var leftClickOnCrossImageContainer = new NSClickGestureRecognizer(this, new ObjCRuntime.Selector(SecondaryActionButtonSelector));
            leftClickOnCrossImageContainer.ButtonMask = LeftButtonMask;
            CrossImageContainer.AddGestureRecognizer(leftClickOnCrossImageContainer);

            SetTitle(LanguageManager.GetLocalizedValue(StringId.PackageUpdaterTitle));

            SetupActionsView();
            SetupDownloadingView();
            SetupInstallingView();
        }

        private void SetupActionsView()
        {
            ActionPageContainer.WantsLayer = true;
            ActionPageContainer.Layer.BackgroundColor = NSColor.Clear.CGColor;

            PageTitleLabel.TextColor = NSColor.White;
            PageSubtitleLabel.TextColor = NSColor.White;

            MainActionButton.WantsLayer = true;
            MainActionButton.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.Mustard).CGColor;
            MainActionButton.Action = new ObjCRuntime.Selector(MainActionButtonSelector);

            SecondaryActionButton.WantsLayer = true;
            SecondaryActionButton.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.Purple).CGColor;
            SecondaryActionButton.Action = new ObjCRuntime.Selector(SecondaryActionButtonSelector);
        }

        private void SetupDownloadingView()
        {
            DownloadingPageContainer.WantsLayer = true;
            DownloadingPageContainer.Layer.BackgroundColor = NSColor.Clear.CGColor;

            DownloadingTitle.TextColor = NSColor.White;
            DownloadingTitle.StringValue = $"{LanguageManager.GetLocalizedValue(StringId.PackageUpdaterDownloadTitle)} (0%)";

            DownloadingProgressBar.Indeterminate = false;
            DownloadingProgressBar.MinValue = 0;
            DownloadingProgressBar.MaxValue = 100;
            DownloadingProgressBar.IncrementBy(50);

            DownloadingCloseButton.WantsLayer = true;
            DownloadingCloseButton.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.Purple).CGColor;
            DownloadingCloseButton.StringValue = LanguageManager.GetLocalizedValue(StringId.CommonClose);
            DownloadingCloseButton.Action = new ObjCRuntime.Selector(SecondaryActionButtonSelector);
        }

        private void SetupInstallingView()
        {
            InstallingPageContainer.WantsLayer = true;
            InstallingPageContainer.Layer.BackgroundColor = NSColor.Clear.CGColor;

            InstallingTitleLabel.TextColor = NSColor.White;
            InstallingTitleLabel.StringValue = "";

            InstallingSubtitleLabel.TextColor = NSColor.White;
            InstallingSubtitleLabel.StringValue = "";

            InstallingSpinner.StartAnimation(null);

            InstallingCloseButton.WantsLayer = true;
            InstallingCloseButton.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.Purple).CGColor;
            InstallingCloseButton.StringValue = LanguageManager.GetLocalizedValue(StringId.CommonClose);
            InstallingCloseButton.Action = new ObjCRuntime.Selector(SecondaryActionButtonSelector);
        }

        private void SetTitle(string title)
        {
            Title = title;
            View.Window.Title = title;
            TitleLabel.StringValue = title;
        }

        private void RefreshUI()
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                DetermineText();
                HideButtonsIfNeeded();
                DetermineViewState();
            });
        }

        /// <summary>
        /// Must be called on UI thread
        /// </summary>
        private void DetermineText()
        {
            var message = MessageFactory.GetMessage(PackageUpdater.State);
            if (message != null)
            {
                //  Action page
                PageTitleLabel.StringValue = message.Title;
                PageSubtitleLabel.StringValue = message.Subtitle;
                MainActionButton.Title = message.MainAction;
                SecondaryActionButton.Title = message.SecondaryAction;

                //  Installing page
                InstallingTitleLabel.StringValue = message.Title;
                InstallingSubtitleLabel.StringValue = message.Subtitle;
                InstallingCloseButton.StringValue = message.SecondaryAction;
            }
        }

        /// <summary>
        /// Must be called on UI thread
        /// </summary>
        private void HideButtonsIfNeeded()
        {
            const int MainActionButtonDefaultWidth = 144;
            const int SecondaryActionButtonDefaultWidth = 90;
            
            if (string.IsNullOrEmpty(MainActionButton.Title))
            {
                ChangeFrameWidth(MainActionButton, 0);
            }
            else
            {
                ChangeFrameWidth(MainActionButton, MainActionButtonDefaultWidth);
            }

            if (string.IsNullOrEmpty(SecondaryActionButton.Title))
            {
                ChangeFrameWidth(SecondaryActionButton, 0);
            }
            else
            {
                ChangeFrameWidth(SecondaryActionButton, SecondaryActionButtonDefaultWidth);
            }

            MainActionButton.NeedsDisplay = true;
            MainActionButton.TranslatesAutoresizingMaskIntoConstraints = true;
            MainActionButton.DisplayIfNeeded();

            SecondaryActionButton.NeedsDisplay = true;
            SecondaryActionButton.TranslatesAutoresizingMaskIntoConstraints = true;
            SecondaryActionButton.DisplayIfNeeded();

            View.NeedsDisplay = true;
            View.TranslatesAutoresizingMaskIntoConstraints = true;
            View.DisplayIfNeeded();
        }

        private void ChangeFrameWidth(NSView view, int width)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            view.Frame = new CGRect(
                view.Frame.X,
                view.Frame.Y,
                width,
                view.Frame.Height
            );
        }

        /// <summary>
        /// Must be called on UI thread
        /// </summary>
        private void DetermineViewState()
        {
            try
            {
                switch (PackageUpdater.State)
                {
                    case PackageUpdateState.WaitUsbKeyboard:
                    case PackageUpdateState.UpdateFailed:
                    case PackageUpdateState.UpdateSucceed:
                    case PackageUpdateState.UpdatePending:
                    case PackageUpdateState.DownloadPending:
                        HideAllPageContainer();
                        ActionPageContainer.Hidden = false;
                        break;
                    case PackageUpdateState.Downloading:
                        HideAllPageContainer();
                        DownloadingPageContainer.Hidden = false;
                        break;
                    case PackageUpdateState.ApplyUpdate:
                        HideAllPageContainer();
                        InstallingPageContainer.Hidden = false;
                        break;
                    case PackageUpdateState.Idle:
                    default:
                        throw new InvalidOperationException("Not supported step");
                }
            }
            catch (InvalidOperationException)
            {
                //  In case of errors we close current alert
                Dismiss();
            }

            View.NeedsLayout = true;
            View.LayoutSubtreeIfNeeded();
        }

        private void HideAllPageContainer()
        {
            DownloadingPageContainer.Hidden = true;
            ActionPageContainer.Hidden = true;
            InstallingPageContainer.Hidden = true;
        }

        private void Dismiss()
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                DidViewDismiss?.Invoke();
                View.Window.Close();
            });
        }

        #region Selectors

        [Export(SecondaryActionButtonSelector)]
        private void OnSecondaryActionPressed()
        {
            if (PackageUpdater.State == PackageUpdateState.UpdateFailed ||
                PackageUpdater.State == PackageUpdateState.UpdateSucceed)
            {
                Task.Run(PackageUpdater.CheckUpdatesAsync);
            }

            Dismiss();
        }

        [Export(MainActionButtonSelector)]
        private void OnMainActionPressed()
        {
            if (PackageUpdater.Component is UpdatableSoftware)
            {
                if (PackageUpdater.State == PackageUpdateState.DownloadPending)
                {
                    OnDownloadButtonPressed();
                }
                else if (PackageUpdater.State == PackageUpdateState.UpdatePending)
                {
                    OnInstallButtonPressed();
                }
            }
            else if (PackageUpdater.Component is UpdatableKeyboard)
            {
                if (PackageUpdater.State == PackageUpdateState.UpdatePending)
                {
                    OnInstallButtonPressed();
                }
            }
            else
            {
                throw new InvalidOperationException("Not manage update type");
            }
        }

        private void OnDownloadButtonPressed()
        {
            Task.Run(async () =>
            {
                try
                {
                    await PackageUpdater?.DownloadPendingUpdateAsync();
                }
                catch(Exception)
                {
                    HideAllPageContainer();
                }
            });
        }

        private void OnInstallButtonPressed()
        {
            Task.Run(async () =>
            {
                try
                {
                    await PackageUpdater?.InstallUpdateAsync();
                }
                catch (Exception exception)
                {
                    HideAllPageContainer();
                }
            });
        }

        [Export("windowShouldClose:")]
        public bool WindowShouldClose(NSObject sender)
        {
            DidViewDismiss?.Invoke();

            return true;
        }

        #endregion

        #region Events

        private void PackageUpdater_StepChanged(object sender, OnStateChangedEventArgs<PackageUpdateState> e) => RefreshUI();

        #endregion

        private void CheckUpdate()
        {
            if (PackageUpdater.Component != null)
            {
                PackageUpdater.Component.OnUpdateStateChanged += Component_OnUpdateStateChanged;
                PackageUpdater.Component.OnUpdateFinished += Component_OnUpdateFinished;
            }
        }

        private void Component_OnUpdateFinished(object sender, Core.PackageUpdater.Updatable.States.UpdateFinishedEventArgs e)
        {
            PackageUpdater.Component.OnUpdateFinished -= Component_OnUpdateFinished;
            PackageUpdater.Component.OnUpdateStateChanged -= Component_OnUpdateStateChanged;
        }

        private void Component_OnUpdateStateChanged(object sender, Core.PackageUpdater.Updatable.States.UpdateStateChangedEventArgs e)
        {
            var state = e.State;
            if (state is UpdateInProgressState progressState)
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    var message = MessageFactory.GetMessage(PackageUpdater.State);
                    var componentName = LanguageManager.GetLocalizedValue(MessageFactory.GetNameForComponent(progressState.Progress.Name));
                    if (progressState.Progress.Name == Core.Keyboard.Updates.Progress.UpdateComponent.Unknown)
                    {
                        InstallingTitleLabel.StringValue = message.Title;
                    }
                    else
                    {
                        InstallingTitleLabel.StringValue = $"{message.Title} : {componentName}";
                    }

                    var doubleValue = progressState.Progress.Percent / 100;
                    var percent = doubleValue.ToString("P0");

                    InstallingPercentLabel.StringValue = percent;
                    InstallingProgressBar.DoubleValue = progressState.Progress.Percent;
                });
            }
            else if (state is UpdateDownloadingState downloadingState)
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    var message = MessageFactory.GetMessage(PackageUpdater.State);
                    var percentFormat = string.Format("{0:00}", downloadingState.Percent);

                    DownloadingProgressBar.DoubleValue = downloadingState.Percent;
                    DownloadingTitle.StringValue = $"{message.Title} {downloadingState.FileIndex + 1}/{downloadingState.FileCount} ({percentFormat}%)";
                });
            }
        }

        private void PackageUpdater_OnUpdateAvailable(object sender, EventArgs e)
        {
            CheckUpdate();
        }
    }
}
