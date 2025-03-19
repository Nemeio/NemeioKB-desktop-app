using System;
using System.Threading.Tasks;
using AppKit;
using CoreGraphics;
using Foundation;
using Microsoft.Extensions.Logging;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.FactoryReset;
using Nemeio.Core.Keyboard.Nemeios.Proxy;
using Nemeio.Core.Managers;
using Nemeio.Core.Services;
using Nemeio.Mac.Models;

namespace Nemeio.Mac.Windows.Alert.FactoryResetModal
{
    public partial class FactoryResetModalController : NSViewController, INSWindowDelegate
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

        private const string ViewStoryboardName = "FactoryResetModal";
        private const string ViewControllerIdentifier = "FactoryResetModalController";

        private const string ValideButtonPressedSelector = "ValidButtonPressed";

        #endregion

        #region Properties

        public IKeyboardController KeyboardController { get; set; }
        public ILanguageManager LanguageManager { get; set; }
        public Action DidViewDismiss { get; set; }
        public IApplicationService ApplicationService { get; set; }
        public ILogger Logger { get; set; }

        #endregion

        #region Constructors

        public static FactoryResetModalController Create(ILoggerFactory loggerFactory, ILanguageManager languageManager, IKeyboardController keyboardController, IApplicationService appService, Action didViewDismiss)
        {
            var storyboard = NSStoryboard.FromName(ViewStoryboardName, null);
            var viewController = storyboard.InstantiateControllerWithIdentifier(ViewControllerIdentifier) as FactoryResetModalController;
            viewController.LanguageManager = languageManager;
            viewController.KeyboardController = keyboardController;
            viewController.DidViewDismiss = didViewDismiss;
            viewController.ApplicationService = appService;
            viewController.Logger = loggerFactory.CreateLogger<FactoryResetModalController>();

            return viewController;
        }

        // Called when created from unmanaged code
        public FactoryResetModalController(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public FactoryResetModalController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public FactoryResetModalController()
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

            SetTitle(LanguageManager.GetLocalizedValue(StringId.FactoryResetTitle));

            TitleLabel.Editable = false;
            TitleLabel.TextColor = MacColorHelper.FromHex(NemeioColor.Green);

            TitleDivider.WantsLayer = true;
            TitleDivider.Layer.BackgroundColor = NSColor.White.CGColor;

            TitleIconContainer.WantsLayer = true;
            TitleIconContainer.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.Purple).CGColor;

            ExplanationLabel.StringValue = LanguageManager.GetLocalizedValue(StringId.FactoryResetExplanation);
            ExplanationLabel.Editable = false;
            ExplanationLabel.TextColor = NSColor.White;

            ValidButton.Action = new ObjCRuntime.Selector(ValideButtonPressedSelector);
            ValidButton.AttributedTitle = new NSAttributedString(
                LanguageManager.GetLocalizedValue(StringId.FactoryResetValidButtonText),
                new NSMutableDictionary() {
                    { NSStringAttributeKey.ForegroundColor, NSColor.White },
                }
            );
            ValidButton.WantsLayer = true;
            ValidButton.Bordered = false;
            ValidButton.BezelStyle = NSBezelStyle.Recessed;
            ValidButton.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.Purple).CGColor;
        }

        private void SetTitle(string title)
        {
            Title = title;
            View.Window.Title = title;
            TitleLabel.StringValue = title;
        }

        #endregion

        #region Selectors

        [Export(ValideButtonPressedSelector)]
        private void OnValidButtonPressed()
        {
            //  We won't that the UI stay blocked
            Task.Run(async () =>
            {
                if (KeyboardController.Connected)
                {
                    var proxy = KeyboardProxy.CastTo<FactoryResetNemeioProxy>(KeyboardController.Nemeio);
                    if (proxy != null)
                    {
                        try
                        {
                            await proxy.ConfirmFactoryResetAsync();
                        }
                        catch (FactoryResetFailedException exception)
                        {
                            Logger.LogError(exception, $"ConfirmFactoryResetAsync failed");
                        }
                    }
                }
            });

            DidViewDismiss();
            View.Window.Close();
        }

        #endregion
    }
}
