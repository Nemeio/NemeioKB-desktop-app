using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AppKit;
using CoreFoundation;
using Foundation;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Icon;
using Nemeio.Core.Managers;
using Nemeio.Core.Services.Layouts;
using Nemeio.Mac.StatusMenu;
using Nemeio.Mac.Windows.Menu;
using Nemeio.Presentation.Menu;

namespace Nemeio.Mac.Services
{
    public class MacTaskBarController : NSObject, IMenuActionHandler
    {
        #region Variables and Properties

        private readonly ILogger _logger;
        private readonly IApplicationIconProvider _iconProvider;
        private readonly IUIMenu _uiMenu;

        private const string StatusItemPressedSelector = "StatusItemPressed";

        private NSPopover _popover;
        private NSStatusItem _statusItem;
        private bool _lastIconIsTemplate;

        public MenuController MenuController { get; private set; }

        #endregion

        #region Constructors

        public MacTaskBarController(ILoggerFactory loggerFactory, IUIMenu uiMenu, IApplicationIconProvider iconProvider, ILanguageManager languageManager)
        {
            MenuController = MenuController.Create(loggerFactory, this, uiMenu, languageManager);

            _logger = loggerFactory.CreateLogger<MacTaskBar>();

            _uiMenu = uiMenu ?? throw new ArgumentNullException(nameof(uiMenu));
            _uiMenu.State.OnValueChanged += MenuState_OnValueChanged;

            _iconProvider = iconProvider ?? throw new ArgumentNullException(nameof(iconProvider));

            _popover = new NSPopover();
            _popover.Behavior = NSPopoverBehavior.Transient;
            _popover.ContentViewController = MenuController;

            _statusItem = NSStatusBar.SystemStatusBar.CreateStatusItem(NSStatusItemLength.Square);
            _statusItem.Menu = null;
            _statusItem.Target = this;
            _statusItem.Action = new ObjCRuntime.Selector(StatusItemPressedSelector);

            RefreshIcon();
        }

        #endregion

        public void ProceedUpdateActionHandler() { }

        public void LayoutSelectedActionHandler(ILayout layout)
        {
            if (layout == null)
            {
                return;
            }

            Task.Run(async () =>
            {
                try
                {
                    await _uiMenu.SelectLayoutAsync(layout);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Enforce System Layout failed");
                }
            });
        }

        #region Icon

        public void RefreshIcon()
        {
            Debug.WriteLine("RefreshIcon");

            bool needTemplate = false;

            if (_uiMenu.Icon.Value != null)
            {
                switch (_uiMenu.Icon.Value.Type)
                {
                    case ApplicationIconType.BluetoothLESyncing:
                    case ApplicationIconType.BluetoothLEConnected:
                    case ApplicationIconType.UpdateAvailable:
                    case ApplicationIconType.UpdateNeeded:
                        needTemplate = false;
                        break;
                    default:
                        needTemplate = true;
                        break;
                }
            }

            var iconName = _iconProvider.GetIconResourceFromCurrentState();

            UpdateIcon(iconName, needTemplate);
        }

        #endregion

        #region Selectors

        [Action(StatusItemPressedSelector)]
        private void StatusItemPressed() => TogglePopover();

        public void UpdateIcon(string iconName, bool template = true)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                _lastIconIsTemplate = template;

                var img = NSImage.ImageNamed(iconName);
                if (img != null)
                {
                    img.Template = template;
                }

                _statusItem.Image = img;
            });
        }

        #endregion

        #region Menu State

        private void MenuState_OnValueChanged(object sender, Presentation.Menu.Tools.ObservableValueChangedEventArgs<MenuState> e)
        {
            switch (e.Value)
            {
                case MenuState.Open:
                    OpenPopover();
                    break;
                case MenuState.Close:
                    ClosePopover();
                    break;
                default:
                    throw new InvalidOperationException($"Value <{e.Value}> not supported");
            }
        }

        #endregion

        private void TogglePopover()
        {
            if (_popover.Shown)
            {
                ClosePopover();
            }
            else
            {
                OpenPopover();
            }

            //  Update icon after selection to avoid MacOS bug
            var oldImage = _statusItem.Image;
            if (oldImage != null)
            {
                oldImage.Template = _lastIconIsTemplate;

                _statusItem.Image = oldImage;
            }
        }

        private void OpenPopover()
        {
            var statusItemButton = _statusItem.Button;
            if (statusItemButton != null)
            {
                _popover.Show(statusItemButton.Bounds, statusItemButton, NSRectEdge.MinYEdge);
            }
        }

        private void ClosePopover() => _popover?.PerformClose(this);
    }
}
