using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AppKit;
using CoreFoundation;
using Foundation;
using Microsoft.Extensions.Logging;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Managers;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.Services.Layouts;
using Nemeio.Mac.Extensions;
using Nemeio.Mac.Icon;
using Nemeio.Mac.Models;
using Nemeio.Mac.StatusMenu;
using Nemeio.Mac.Views;
using Nemeio.Presentation.Menu;
using Nemeio.Presentation.Menu.Battery;
using Nemeio.Presentation.Menu.Configurator;
using Nemeio.Presentation.Menu.Connection;
using Nemeio.Presentation.Menu.Layouts;
using Nemeio.Presentation.Menu.Quit;
using Nemeio.Presentation.Menu.Synchronization;
using Nemeio.Presentation.Menu.Version;

namespace Nemeio.Mac.Windows.Menu
{
    public partial class MenuController : NSViewController, INSTableViewDelegate, INSTableViewDataSource, IAssociatedApplicationDelegate
    {
        #region Variables, Properties, Constantes

        private nuint LeftButtonMask = 0x1;
        private nuint RightButtonMask = 0x2;

        private const string MenuKeyboardManagerPressedSelector = "KeyboardManagerPressed";
        private const string MenuQuitPressedSelector = "QuitPressed";
        private const string MenuLeftVersionPressedSelector = "LeftVersionPressed";
        private const string MenuRightVersionPressedSelector = "RightVersionPressed";

        private const string MenuStoryboardName = "Menu";
        private const string MenuViewControllerIdentifier = "MenuController";

        private const string VeryLowBatteryImageName = "Battery-20";
        private const string LowBatteryImageName = "Battery-40";
        private const string MediumBatteryImageName = "Battery-60";
        private const string HighBatteryImageName = "Battery-80";
        private const string MaximumBatteryImageName = "Battery-100";

        private const string CommunicationUsbImageName = "communication_mode_usb";
        private const string CommunicationBleImageName = "communication_mode_ble";

        private const int DividerCellHeight = 16;
        private const int KeyboardCellHeight = 32;
        private const int MenuItemHeight = 44;

        private const int DefaultTitleHeight = 14;
        private const int SmallTitleHeight = 12;

        private IList<ILayout> _layouts = new List<ILayout>();
        private IList<TableViewRow> _rows;

        public ILoggerFactory LoggerFactory { get; private set; }
        public IMenuActionHandler MenuActionHandler { get; set; }
        public IUIMenu UIMenu { get; set; }
        public ILanguageManager LanguageManager { get; set; }

        #endregion

        #region Constructor

        public static MenuController Create(ILoggerFactory loggerFactory, IMenuActionHandler menuHandler, IUIMenu uiMenu, ILanguageManager languageManager)
        {
            var storyboard = NSStoryboard.FromName(MenuStoryboardName, null);

            var viewController = storyboard.InstantiateControllerWithIdentifier(MenuViewControllerIdentifier) as MenuController;
            viewController.LoggerFactory = loggerFactory;
            viewController.UIMenu = uiMenu;
            viewController.MenuActionHandler = menuHandler;
            viewController.LanguageManager = languageManager;

            return viewController;
        }

        public MenuController(IntPtr handle)
            : base(handle) { }

        #endregion

        public override void ViewWillAppear()
        {
            base.ViewWillAppear();

            SetupView();
        }

        public override void ViewDidAppear()
        {
            base.ViewDidAppear();

            RefreshMenu();
        }

        public void RefreshMenu()
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                RefreshQuitSection(UIMenu.Menu.Value.Quit);
                RefreshConfiguratorSection(UIMenu.Menu.Value.Configurator);
                RefreshLayouts(UIMenu.Menu.Value.Layouts);
                RefreshVersion(UIMenu.Menu.Value.Versions);
                RefreshBattery(UIMenu.Menu.Value.Battery);
                RefreshSyncInProgress(UIMenu.Menu.Value.Synchronization);
                RefreshConnectedMenu(UIMenu.Menu.Value.Connection);
            });
        }

        #region Version

        [Export(MenuLeftVersionPressedSelector)]
        private void OnLeftVersionPressed() => UIMenu?.DisplayUpdateModalIfNeeded();

        [Export(MenuRightVersionPressedSelector)]
        private void OnRightVersionPressed()
        {
            var versionSection = UIMenu.Menu.Value.Versions;

            var menu = new NSMenu();
            menu.AddItem(versionSection.Title, null, "");
            menu.AddItem($"{versionSection.ApplicationVersionTitle}", null, "");

            if (versionSection.KeyboardIsPlugged)
            {
                menu.AddItem($"{versionSection.Stm32VersionTitle}", null, "");
                menu.AddItem($"{versionSection.BluetoothLEVersionTitle}", null, "");
                menu.AddItem($"{versionSection.IteVersionTitle}", null, "");
            }

            menu.PopUpMenu(null, NSEvent.CurrentMouseLocation, null);
        }

        public void RefreshVersion(VersionSection section)
        {
            if (section == null)
            {
                return;
            }

            var versionText = new NSMutableAttributedString(section.Title);
            versionText.AddAttribute(NSStringAttributeKey.ForegroundColor, NSColor.White, new NSRange(0, versionText.Length));
            versionText.AddAttribute(NSStringAttributeKey.Font, MacFontHelper.GetOpenSans(12), new NSRange(0, versionText.Length));

            if (VersionRow != null)
            {
                VersionRowLabel.Editable = false;
                VersionRowLabel.AttributedStringValue = versionText;
                VersionRowLabel.Alignment = NSTextAlignment.Left;
            }

            var updateStateValue = section.UpdateStatus;
            var updateStateColor = MacColorHelper.FromHex(NemeioColor.Green);

            switch (section.UpdateKind)
            {
                case PackageUpdateState.UpdateChecking:
                case PackageUpdateState.CheckApplicationUpdate:
                case PackageUpdateState.CheckFirmwareUpdate:
                case PackageUpdateState.CheckInternetConnection:
                case PackageUpdateState.Idle:
                case PackageUpdateState.UpdateSucceed:
                    updateStateColor = MacColorHelper.FromHex(NemeioColor.Green);
                    break;
                case PackageUpdateState.DownloadPending:
                case PackageUpdateState.ApplyUpdate:
                    updateStateColor = MacColorHelper.FromHex(NemeioColor.Mustard);
                    break;
                case PackageUpdateState.UpdatePending:
                case PackageUpdateState.UpdateFailed:
                    updateStateColor = MacColorHelper.FromHex(NemeioColor.Red);
                    break;
                default:
                    throw new InvalidOperationException("Unknown update kind");
            }

            var paragraphStyle = new NSMutableParagraphStyle();
            paragraphStyle.Alignment = NSTextAlignment.Right;

            var updatedText = new NSMutableAttributedString(updateStateValue);
            updatedText.AddAttribute(NSStringAttributeKey.ForegroundColor, updateStateColor, new NSRange(0, updatedText.Length));
            updatedText.AddAttribute(NSStringAttributeKey.Font, MacFontHelper.GetOpenSans(10), new NSRange(0, updatedText.Length));
            updatedText.AddAttribute(NSStringAttributeKey.ParagraphStyle, paragraphStyle, new NSRange(0, updatedText.Length));

            if (VersionRow != null)
            {
                VersionRowStateLabel.Editable = false;
                VersionRowStateLabel.AttributedStringValue = updatedText;
            }
        }

        #endregion

        #region Synchronization

        public void RefreshSyncInProgress(SynchronizationSection section)
        {
            if (section != null)
            {
                if (KeyboardOverlay != null)
                {
                    KeyboardOverlay.Hidden = !section.Visible;
                }

                if (section.Visible)
                {
                    var syncText = new NSMutableAttributedString(section.Title);
                    syncText.AddAttribute(NSStringAttributeKey.ForegroundColor, NSColor.White, new NSRange(0, syncText.Length));
                    syncText.AddAttribute(NSStringAttributeKey.Font, MacFontHelper.GetOpenSans(12), new NSRange(0, syncText.Length));

                    var stepText = new NSMutableAttributedString(section.ProgressDescription);
                    stepText.AddAttribute(NSStringAttributeKey.ForegroundColor, MacColorHelper.FromHex(NemeioColor.Mustard), new NSRange(0, stepText.Length));
                    stepText.AddAttribute(NSStringAttributeKey.Font, MacFontHelper.GetOpenSans(12), new NSRange(0, stepText.Length));

                    if (SyncInProgressRow != null)
                    {
                        SyncInProgressTitleLabel.AttributedStringValue = syncText;
                        SyncInProgressValueLabel.AttributedStringValue = stepText;
                        SyncInProgressRow.Maximize(MenuItemHeight);
                    }
                }
                else
                {
                    if (SyncInProgressRow != null)
                    {
                        SyncInProgressRow.Minimize();
                        SyncInProgressTitleLabel.StringValue = string.Empty;
                        SyncInProgressValueLabel.StringValue = string.Empty;
                    }
                }
            }
            else
            {
                if (SyncInProgressRow != null)
                {
                    SyncInProgressRow.Minimize();
                    SyncInProgressTitleLabel.StringValue = string.Empty;
                    SyncInProgressValueLabel.StringValue = string.Empty;
                }
            }
        }

        #endregion

        #region Quit

        [Export(MenuQuitPressedSelector)]
        private void OnQuitPressed() => UIMenu.DisplayQuitModal();

        public void RefreshQuitSection(QuitSection section)
        {
            if (QuitRowLabel != null && section != null)
            {
                QuitRowLabel.StringValue = section.Title;
            }
        }

        #endregion

        #region Configurator

        [Export(MenuKeyboardManagerPressedSelector)]
        private void OnOpenKeyboardManagerPressed() => UIMenu.DisplayConfiguratorModal();

        public void RefreshConfiguratorSection(ConfiguratorSection section)
        {
            if (KeyboardManagerRowLabel != null && section != null)
            {
                KeyboardManagerRowLabel.StringValue = section.Title;
            }
        }

        #endregion

        #region Battery

        public void RefreshBattery(BatterySection section)
        {
            if (BatteryRow == null || section == null)
            {
                return;
            }

            if (!section.Visible)
            {
                BatteryRow.Minimize();

                return;
            }

            BatteryRow.Maximize(MenuItemHeight);
            BatteryRowIconImageView.Image = GetBatteryImageByType(section.Image);
            BatteryRowTitleLabel.StringValue = section.Text;
        }

        private NSImage GetBatteryImageByType(BatteryImageType type)
        {
            switch (type)
            {
                case BatteryImageType.Level20:
                    return NSImage.ImageNamed(VeryLowBatteryImageName);
                case BatteryImageType.Level40:
                    return NSImage.ImageNamed(LowBatteryImageName);
                case BatteryImageType.Level60:
                    return NSImage.ImageNamed(MediumBatteryImageName);
                case BatteryImageType.Level80:
                    return NSImage.ImageNamed(HighBatteryImageName);
                case BatteryImageType.Level100:
                default:
                    return NSImage.ImageNamed(MaximumBatteryImageName);
            }
        }

        #endregion

        #region Connection

        public void RefreshConnectedMenu(ConnectionSection section)
        {
            if (section != null && section.Visible)
            {
                if (ConnectedRowView == null)
                {
                    return;
                }

                var connectedIcon = section.Communication == CommunicationType.Serial ? CommunicationUsbImageName : CommunicationBleImageName;

                ConnectedIconImageView.Image = NSImage.ImageNamed(connectedIcon);
                ConnectedTitleLabel.StringValue = section.Title;
                ConnectedRowView.Maximize(MenuItemHeight);
            }
            else
            {
                ConnectedRowView?.Minimize();
            }
        }

        #endregion

        #region Layouts

        private const int LayoutListMaxSize = 300;

        private void OnLayoutSelected(ILayout layout) => MenuActionHandler.LayoutSelectedActionHandler(layout);

        public void RefreshLayouts(LayoutsSection section)
        {
            if (section != null && section.Visible)
            {
                DisplayLayoutsList();
                BuildDataModel(section);
                KeyboardTableView?.ReloadData();
            }
            else
            {
                HideLayoutsList();
            }
        }

        private void DisplayLayoutsList()
        {
            LayoutsContainerView?.Maximize(LayoutListMaxSize);
        }

        private void HideLayoutsList()
        {
            LayoutsContainerView?.Minimize();
        }

        private void BuildDataModel(LayoutsSection section)
        {
            _rows = new List<TableViewRow>();

            var customLayouts = section.Subsections.Where(x => x.IsStandard == false).ToList();
            if (customLayouts.Count() > 0)
            {
                _rows.Add(
                    new TableViewRow()
                    {
                        AssociatedObject = LanguageManager.GetLocalizedValue(StringId.CommonCustom),
                        CellIdentifier = MenuTableViewCellIdentifier.Divider
                    }
                );

                AddLayoutsToRows(customLayouts);
            }
            var hidLayouts = section.Subsections.Where(x => x.IsStandard == true).ToList();
            if (hidLayouts.Count() > 0)
            {
                _rows.Add(
                    new TableViewRow()
                    {
                        AssociatedObject = LanguageManager.GetLocalizedValue(StringId.CommonStandard),
                        CellIdentifier = MenuTableViewCellIdentifier.Divider
                    }
                );

                AddLayoutsToRows(hidLayouts);
            }

        }

        private void AddLayoutsToRows(IEnumerable<LayoutSubsection> subsections)
        {
            foreach (var subsection in subsections)
            {
                _rows.Add(
                    new KeyboadTableViewRow()
                    {
                        AssociatedObject = subsection.Layout,
                        CellIdentifier = MenuTableViewCellIdentifier.Keyboard,
                        Selected = subsection.IsSelected,
                        ApplicationAugmentedHidEnabled = subsection.AugmentedHidEnabled
                    }
                );
            }
        }

        #endregion

        #region TableView

        #region TableView Delegate

        [Export("tableView:heightOfRow:")]
        public nfloat GetRowHeight(NSTableView tableView, nint row)
        {
            var tableViewRow = _rows.ElementAt((int)row);

            return tableViewRow.CellIdentifier == MenuTableViewCellIdentifier.Divider ? DividerCellHeight : KeyboardCellHeight;
        }

        [Export("tableView:shouldSelectRow:")]
        public bool ShouldSelectRow(NSTableView tableView, nint row)
        {
            //  Can select row only if we are connected and not syncing

            var syncing = false;
            if (UIMenu.Menu.Value.Synchronization != null)
            {
                syncing = UIMenu.Menu.Value.Synchronization.Visible;
            }

            var connected = false;
            if (UIMenu.Menu.Value.Connection != null)
            {
                connected = UIMenu.Menu.Value.Connection.Visible;
            }

            return !syncing && connected;
        }

        [Export("tableViewSelectionDidChange:")]
        public void SelectionDidChange(NSNotification notification)
        {
            var tableViewSelectedRow = (int)KeyboardTableView.SelectedRow;
            var currentRow = _rows.ElementAt(tableViewSelectedRow);
            var layout = currentRow.AssociatedObject as ILayout;

            if (layout != null)
            {
                RefreshLayouts(UIMenu.Menu.Value.Layouts);
                OnLayoutSelected(layout);
            }
        }

        [Export("tableView:viewForTableColumn:row:")]
        public NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            var currentRow = _rows.ElementAt((int)row);
            if (currentRow != null)
            {
                var cell = tableView.MakeView(currentRow.CellIdentifier.ToString(), this) as BaseTableViewCell;
                cell.FromController = this;
                cell.LanguageManager = LanguageManager;
                cell.Setup(currentRow);

                return cell;
            }

            return null;
        }

        #endregion

        #region TableView DataSource

        [Export("numberOfRowsInTableView:")]
        public nint GetRowCount(NSTableView tableView) => _rows?.Count() ?? 0;

        #endregion

        #endregion

        #region Associated Application Delegate

        public void KeyboardAssociatedApplicationClicked(ILayout layout)
        {
            layout.LayoutInfo.LinkApplicationEnable = !layout.LayoutInfo.LinkApplicationEnable;

            RefreshLayouts(UIMenu.Menu.Value.Layouts);
        }

        #endregion

        private void SetupView()
        {
            if (BodyStackView == null)
            {
                //  View is not construct, we bypass setup
                return;
            }

            BodyStackView.WantsLayer = true;
            BodyStackView.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.DarkPurple).CGColor;

            QuitRowIconContainer.WantsLayer = true;
            QuitRowIconContainer.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.Purple).CGColor;

            QuitRowLabel.TextColor = NSColor.White;
            QuitRowLabel.Editable = false;
            QuitRowLabel.Selectable = false;
            QuitRowLabel.Font = MacFontHelper.GetOpenSans(DefaultTitleHeight);

            QuitRowDivider.WantsLayer = true;
            QuitRowDivider.Layer.BackgroundColor = NSColor.White.CGColor;

            var icon = NSImage.ImageNamed("connected_keyboard");
            icon.Template = false;

            KeyboardManagerRowImageView.AllowsCutCopyPaste = false;
            KeyboardManagerRowImageView.RefusesFirstResponder = true;
            KeyboardManagerRowImageView.Image = icon;
            KeyboardManagerRowImageView.ImageFrameStyle = NSImageFrameStyle.None;
            KeyboardManagerRowImageView.WantsLayer = true;
            KeyboardManagerRowImageView.Layer.BackgroundColor = NSColor.Clear.CGColor;

            KeyboardManagerRowIconContainer.WantsLayer = true;
            KeyboardManagerRowIconContainer.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.Purple).CGColor;

            KeyboardManagerRowLabel.TextColor = MacColorHelper.FromHex(NemeioColor.Green);
            KeyboardManagerRowLabel.Editable = false;
            KeyboardManagerRowLabel.Selectable = false;
            KeyboardManagerRowLabel.Font = MacFontHelper.GetOpenSans(DefaultTitleHeight);

            KeyboardManagerRowDivider.WantsLayer = true;
            KeyboardManagerRowDivider.Layer.BackgroundColor = NSColor.White.CGColor;

            VersionRowDivider.WantsLayer = true;
            VersionRowDivider.Layer.BackgroundColor = NSColor.White.CGColor;

            VersionRowLabel.TextColor = NSColor.White;
            VersionRowLabel.Editable = false;
            VersionRowLabel.Selectable = false;
            VersionRowLabel.Alignment = NSTextAlignment.Left;
            VersionRowLabel.MaximumNumberOfLines = 1;
            VersionRowLabel.LineBreakMode = NSLineBreakMode.TruncatingTail;
            VersionRowStateLabel.Editable = false;
            VersionRowStateLabel.Alignment = NSTextAlignment.Left;

            KeyboardTableView.HeaderView = null;
            KeyboardTableView.BackgroundColor = MacColorHelper.FromHex(NemeioColor.DarkPurple);
            KeyboardTableView.Delegate = this;
            KeyboardTableView.DataSource = this;
            KeyboardTableView.AllowsMultipleSelection = false;
            KeyboardTableView.EnclosingScrollView.HasVerticalScroller = false;
            KeyboardTableView.WantsLayer = true;
            KeyboardTableView.Layer.BorderColor = NSColor.Clear.CGColor;
            KeyboardTableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.None;

            KeyboardOverlay.WantsLayer = true;
            KeyboardOverlay.Layer.BackgroundColor = MacColorHelper.FromHex(NemeioColor.DarkPurple).CGColor;
            KeyboardOverlay.Layer.Opacity = 0.8f;
            KeyboardOverlay.Hidden = false;

            BatteryRowTitleLabel.TextColor = NSColor.White;
            BatteryRowTitleLabel.Editable = false;
            BatteryRowTitleLabel.Selectable = false;
            BatteryRowTitleLabel.StringValue = "---%";
            BatteryRowTitleLabel.Font = MacFontHelper.GetOpenSans(14);

            ConnectedTitleLabel.TextColor = NSColor.White;
            ConnectedTitleLabel.Editable = false;
            ConnectedTitleLabel.Selectable = false;
            ConnectedTitleLabel.StringValue = "???";
            ConnectedTitleLabel.Font = MacFontHelper.GetOpenSans(14);

            ConnectedIconImageView.AllowsCutCopyPaste = false;
            ConnectedIconImageView.RefusesFirstResponder = true;
            ConnectedIconImageView.ImageFrameStyle = NSImageFrameStyle.None;

            var syncIcon = NSImage.ImageNamed(MacApplicationIconProvider.NemeioDisconnectedImageName);
            syncIcon.Template = false;

            SyncIconImageView.Image = syncIcon;
            SyncIconImageView.AllowsCutCopyPaste = false;
            SyncIconImageView.RefusesFirstResponder = true;
            SyncIconImageView.ImageFrameStyle = NSImageFrameStyle.None;

            SyncInProgressTitleLabel.Editable = false;
            SyncInProgressTitleLabel.Selectable = false;
            SyncInProgressTitleLabel.Alignment = NSTextAlignment.Left;
            SyncInProgressTitleLabel.MaximumNumberOfLines = 1;
            SyncInProgressTitleLabel.LineBreakMode = NSLineBreakMode.TruncatingTail;
            SyncInProgressValueLabel.Editable = false;
            SyncInProgressValueLabel.Alignment = NSTextAlignment.Left;

            DebugRowView.Hidden = true;
            /*
            DebugIconImageView.AllowsCutCopyPaste = false;
            DebugIconImageView.RefusesFirstResponder = true;
            DebugIconImageView.ImageFrameStyle = NSImageFrameStyle.None;

            DebugTitleLabel.Editable = false;
            DebugTitleLabel.Selectable = false;
            DebugTitleLabel.TextColor = NSColor.White;
            DebugTitleLabel.Font = MacFontHelper.GetOpenSans(SmallTitleHeight);
            DebugTitleLabel.StringValue = "DEBUG: Resync layouts";
            */

            var rightClickOnVersion = new NSClickGestureRecognizer(this, new ObjCRuntime.Selector(MenuRightVersionPressedSelector));
            rightClickOnVersion.ButtonMask = RightButtonMask;
            VersionRow.AddGestureRecognizer(rightClickOnVersion);

            var leftClickOnVersion = new NSClickGestureRecognizer(this, new ObjCRuntime.Selector(MenuLeftVersionPressedSelector));
            leftClickOnVersion.ButtonMask = LeftButtonMask;
            VersionRow.AddGestureRecognizer(leftClickOnVersion);

            KeyboardManagerRow.AddGestureRecognizer(new NSClickGestureRecognizer(this, new ObjCRuntime.Selector(MenuKeyboardManagerPressedSelector)));
            QuitRow.AddGestureRecognizer(new NSClickGestureRecognizer(this, new ObjCRuntime.Selector(MenuQuitPressedSelector)));
        }
    }
}
