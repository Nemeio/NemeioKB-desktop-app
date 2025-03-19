using System;
using System.Globalization;
using System.Linq;
using AppKit;
using CoreGraphics;
using Foundation;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Managers;
using Nemeio.Mac.Models;

namespace Nemeio.Mac.Windows.Alert.LanguageSelection
{
    public partial class LanguageSelectionController : NSViewController, INSWindowDelegate
    {
        private const string ViewStoryboardName = "LanguageSelection";
        private const string ViewControllerIdentifier = "LanguageSelectionController";
        private const string ValideButtonPressedSelector = "ValidButtonPressed";

        private NSComboBoxDataController _comboBoxDataController;
        private CultureInfo _selectedLanguage;

        public ILanguageManager LanguageManager { get; set; }
        public Action DidViewDismiss { get; set; }

        #region Constructors

        public static LanguageSelectionController Create(ILanguageManager languageManager, Action didViewDismiss)
        {
            var storyboard = NSStoryboard.FromName(ViewStoryboardName, null);
            var viewController = storyboard.InstantiateControllerWithIdentifier(ViewControllerIdentifier) as LanguageSelectionController;
            viewController.LanguageManager = languageManager;
            viewController.DidViewDismiss = didViewDismiss;

            return viewController;
        }

        // Called when created from unmanaged code
        public LanguageSelectionController(IntPtr handle)
            : base(handle) { }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public LanguageSelectionController(NSCoder coder)
            : base(coder) { }

        #endregion

        #region Lifecycle Methods

        public override void ViewWillAppear()
        {
            base.ViewWillAppear();
            SetupView();
            LoadData();
            PreferredContentSize = new CGSize(480, 260);
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

            SetTitle(LanguageManager.GetLocalizedValue(StringId.SelectLanguageTitle));

            TitleLabel.Editable = false;
            TitleLabel.TextColor = MacColorHelper.FromHex(NemeioColor.Green);

            TitleDivider.WantsLayer = true;
            TitleDivider.Layer.BackgroundColor = NSColor.White.CGColor;

            TitleIconContainer.WantsLayer = true;
            TitleIconContainer.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.Purple).CGColor;

            InformationLabel.StringValue = LanguageManager.GetLocalizedValue(StringId.SelectLanguageInformationText);
            InformationLabel.Editable = false;
            InformationLabel.TextColor = NSColor.White;

            ValidButton.Action = new ObjCRuntime.Selector(ValideButtonPressedSelector);
            ValidButton.AttributedTitle = new NSAttributedString(
                LanguageManager.GetLocalizedValue(StringId.SelectLanguageValidButton),
                new NSMutableDictionary() {
                    { NSStringAttributeKey.ForegroundColor, NSColor.White },
                }
            );

            ValidButton.WantsLayer = true;
            ValidButton.Bordered = false;
            ValidButton.BezelStyle = NSBezelStyle.Recessed;
            ValidButton.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.Purple).CGColor;
            ValidButton.Enabled = false;
        }

        private void SetTitle(string title)
        {
            Title = title;
            View.Window.Title = title;
            TitleLabel.StringValue = title;
        }

        private void LoadData()
        {
            var languages = LanguageManager.GetSupportedLanguages()
                                            .Select(x => x.NativeName);

            _comboBoxDataController = new NSComboBoxDataController(languages, LanguageSelectionChanged);

            LanguagesComboBox.UsesDataSource = true;
            LanguagesComboBox.DataSource = _comboBoxDataController;
            LanguagesComboBox.Delegate = _comboBoxDataController;
        }

        private void LanguageSelectionChanged(int itemIndex)
        {
            _selectedLanguage = LanguageManager
                                    .GetSupportedLanguages()
                                    .ElementAt(itemIndex);

            ValidButton.Enabled = _selectedLanguage != null;
        }

        #endregion

        #region Selectors

        [Export(ValideButtonPressedSelector)]
        private void OnValidButtonPressed()
        {
            LanguageManager.SelectMainLanguage(_selectedLanguage);
            DidViewDismiss();
            View.Window.Close();
        }

        [Export("windowShouldClose:")]
        public bool WindowShouldClose(NSObject sender)
        {
            //  Could not exit this window

            return false;
        }

        #endregion
    }
}
