using System;
using AppKit;
using CoreGraphics;
using Foundation;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Managers;
using Nemeio.Mac.Models;

namespace Nemeio.Mac.Windows.Alert.KeyboardInitErrorModal
{
    public partial class KeyboardInitErrorModalController : NSViewController, INSWindowDelegate
    {
        #region Variables

        private const string ViewStoryboardName = "KeyboardInitErrorModal";
        private const string ViewControllerIdentifier = "KeyboardInitErrorModalController";

        private const string CloseButtonPressedSelector = "CloseButtonPressed";

        #endregion

        #region Properties

        public ILanguageManager LanguageManager { get; set; }
        public Action DidViewDismiss { get; set; }

        #endregion

        #region Constructors

        public static KeyboardInitErrorModalController Create(ILanguageManager languageManager, Action didViewDismiss)
        {
            var storyboard = NSStoryboard.FromName(ViewStoryboardName, null);
            var viewController = storyboard.InstantiateControllerWithIdentifier(ViewControllerIdentifier) as KeyboardInitErrorModalController;
            viewController.DidViewDismiss = didViewDismiss;
            viewController.LanguageManager = languageManager;

            return viewController;
        }

        // Called when created from unmanaged code
        public KeyboardInitErrorModalController(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public KeyboardInitErrorModalController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public KeyboardInitErrorModalController()
            : base("FactoryResetModal", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        #region Lifecycle Methods

        public override void ViewWillAppear()
        {
            base.ViewWillAppear();
            SetupView();
            PreferredContentSize = new CGSize(540, 280);
        }

        public override void ViewDidAppear()
        {
            base.ViewDidAppear();
            View.Window.StyleMask = NSWindowStyle.Closable | NSWindowStyle.Titled;
            View.Window.Delegate = this;
        }

        #endregion

        #region Methods

        private void SetupView()
        {
            View.WantsLayer = true;
            View.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.DarkPurple).CGColor;

            SetTitle(LanguageManager.GetLocalizedValue(StringId.KeyboardInitFailedModalTitle));

            TitleLabel.Editable = false;
            TitleLabel.TextColor = MacColorHelper.FromHex(NemeioColor.Green);

            TitleDivider.WantsLayer = true;
            TitleDivider.Layer.BackgroundColor = NSColor.White.CGColor;

            TitleIconContainer.WantsLayer = true;
            TitleIconContainer.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.Purple).CGColor;

            ExplanationLabel.StringValue = LanguageManager.GetLocalizedValue(StringId.KeyboardInitFailedModalExplanation);
            ExplanationLabel.Editable = false;
            ExplanationLabel.TextColor = NSColor.White;

            CloseButton.Action = new ObjCRuntime.Selector(CloseButtonPressedSelector);
            CloseButton.AttributedTitle = new NSAttributedString(
                LanguageManager.GetLocalizedValue(StringId.KeyboardInitFailedModalCloseButtonText),
                new NSMutableDictionary() {
                    { NSStringAttributeKey.ForegroundColor, NSColor.White },
                }
            );
            CloseButton.WantsLayer = true;
            CloseButton.Bordered = false;
            CloseButton.BezelStyle = NSBezelStyle.Recessed;
            CloseButton.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.Purple).CGColor;
        }

        private void SetTitle(string title)
        {
            Title = title;
            View.Window.Title = title;
            TitleLabel.StringValue = title;
        }

        #endregion

        #region Selectors

        [Export(CloseButtonPressedSelector)]
        private void OnCloseButtonPressed()
        {
            DidViewDismiss();
            View.Window.Close();
        }

        #endregion
    }
}
