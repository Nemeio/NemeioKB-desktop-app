using System;
using AppKit;
using Foundation;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Managers;
using Nemeio.Mac.Models;

namespace Nemeio.Mac.Windows.Alert
{
    public partial class QuitViewController : NSViewController, INSWindowDelegate
    {
        private const string AlertStoryboardName                = "Quit";
        private const string AlertViewControllerIdentifier      = "QuitViewController";

        private const string PositiveButtonPressedSelector      = "PositiveButtonPressed";
        private const string NegativeButtonPressedSelector      = "NegativeButtonPressed";

        public ILanguageManager LanguageManager { get; set; }
        public Action<bool> DidDismiss { get; private set; }

        #region Constructors

        public static QuitViewController Create(ILanguageManager languageManager, Action<bool> dismissAction)
        {
            var storyboard = NSStoryboard.FromName(AlertStoryboardName, null);
            var viewController = storyboard.InstantiateControllerWithIdentifier(AlertViewControllerIdentifier) as QuitViewController;
            viewController.LanguageManager = languageManager;
            viewController.DidDismiss = dismissAction;

            return viewController;
        }

        // Called when created from unmanaged code
        public QuitViewController(IntPtr handle)
            : base(handle) { }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public QuitViewController(NSCoder coder)
            : base(coder) { }

        // Call to load from the XIB/NIB file
        public QuitViewController()
            : base("AlertViewController", NSBundle.MainBundle) { }

        #endregion

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupView();
            SetTitle(LanguageManager.GetLocalizedValue(StringId.CommonButtonQuit));
        }

        public override void ViewDidAppear()
        {
            base.ViewDidAppear();
            View.Window.StyleMask = NSWindowStyle.Closable | NSWindowStyle.Titled;
            View.Window.Delegate = this;
        }

        private void SetupView()
        {
            View.WantsLayer = true;
            View.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.DarkPurple).CGColor;

            TitleLabel.StringValue = "...";
            TitleLabel.Editable = false;
            TitleLabel.TextColor = MacColorHelper.FromHex(NemeioColor.Green);

            TitleDivider.WantsLayer = true;
            TitleDivider.Layer.BackgroundColor = NSColor.White.CGColor;

            TitleIconContainer.WantsLayer = true;
            TitleIconContainer.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.Purple).CGColor;

            PositiveButton.Action = new ObjCRuntime.Selector(PositiveButtonPressedSelector);
            CustomizeButton(
                PositiveButton,
                LanguageManager.GetLocalizedValue(StringId.CommonYes)
            );

            NegativeButton.Action = new ObjCRuntime.Selector(NegativeButtonPressedSelector);
            CustomizeButton(
                NegativeButton,
                LanguageManager.GetLocalizedValue(StringId.CommonNo)
            );

            MessageLabel.StringValue = LanguageManager.GetLocalizedValue(StringId.QuitQuestion);
            MessageLabel.Alignment = NSTextAlignment.Center;
            MessageLabel.Editable = false;
            MessageLabel.TextColor = NSColor.White;

            InformationLabel.StringValue = LanguageManager.GetLocalizedValue(StringId.QuitDisclaimer);
            InformationLabel.Alignment = NSTextAlignment.Center;
            InformationLabel.Editable = false;
            InformationLabel.TextColor = NSColor.White;
        }

        private void CustomizeButton(NSButton button, string title)
        {
            button.WantsLayer = true;
            button.Bordered = false;
            button.BezelStyle = NSBezelStyle.Recessed;
            button.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.Purple).CGColor;
            button.AttributedTitle = new NSAttributedString(
                title,
                new NSMutableDictionary() {
                    { NSStringAttributeKey.ForegroundColor, NSColor.White },
                }
            );
        }

        private void SetTitle(string title)
        {
            TitleLabel.StringValue = title;
            Title = title;
        }

        [Export(PositiveButtonPressedSelector)]
        private void OnPositiveButtonPressed()
        {
            DidDismiss(true);
            View.Window.Close();
        }

        [Export(NegativeButtonPressedSelector)]
        private void OnNegativeButtonPressed()
        {
            DidDismiss(false);
            DismissViewController(this);
        }

        [Export("windowShouldClose:")]
        public bool WindowShouldClose(NSObject sender)
        {
            OnNegativeButtonPressed();

            return true;
        }
    }
}
