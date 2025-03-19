using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Hardcodet.Wpf.TaskbarNotification;
using Hardcodet.Wpf.TaskbarNotification.Interop;
using Microsoft.Extensions.Logging;
using MvvmCross.Platform;
using Nemeio.Presentation;
using Nemeio.Windows.Win32;
using Nemeio.Wpf.ViewModel;

namespace Nemeio.Wpf.UserControls
{
    using static Nemeio.Windows.Win32.WinUser32;
    using ReflectionBindingFlags = System.Reflection.BindingFlags;
    using WinPoint = System.Windows.Point;

    /// <summary>
    /// Interaction logic for TaskBarIconMenuView.xaml
    /// </summary>
    public partial class TaskBarIconMenuView : UserControl
    {
        private readonly ILogger _logger;
        private readonly TaskBarIconMenuViewModel TaskBarIconInstanceMenuViewModel;
        private SemaphoreSlim _layoutSelectionSemaphore;

        private enum TaskBarLocation { Top, Bottom, Left, Right }
        private const double absoluteTrayIconWidth = 40;
        private const double absoluteTrayIconHeigt = 24;

        public TaskBarIconMenuView()
        {
            InitializeComponent();
            TaskBarIconInstanceMenuViewModel = Mvx.Resolve<IMainUserInterface>() as TaskBarIconMenuViewModel;
            _logger = Mvx.Resolve<ILoggerFactory>().CreateLogger<TaskBarIconMenuView>();
            _layoutSelectionSemaphore = new SemaphoreSlim(1, 1);
        }

        private async void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e != null && e.AddedItems.Count == 0)
            {
                //  We don't have to handle event with no change

                return;
            }

            await _layoutSelectionSemaphore.WaitAsync();

            try 
            {
                if (e != null && e.AddedItems.Count == 1)
                {
                    var selected = e.AddedItems[0] as LayoutViewModel;

                    _logger.LogInformation($"User select item on list <{selected.Layout.LayoutId}>");

                    if (!selected.Enabled)
                    {
                        return;
                    }

                    await TaskBarIconInstanceMenuViewModel.SetLayoutSelectionAsync(selected.Layout);

                    if (sender == customListView)
                    {
                        standardListView.SelectedItem = null;
                    }
                    else if (sender == standardListView)
                    {
                        customListView.SelectedItem = null;
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Change layout failed");
            }
            finally 
            {
                _layoutSelectionSemaphore.Release();
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChangeControlPosition();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ChangeControlPosition();
        }

        private void ChangeControlPosition()
        {
            // get tray icon position
            var trayIconRectangle = GetParentTrayIconPosition();

            // adjust menu position relative to tray icon
            var menuPosition = GetMenuStartPoint(trayIconRectangle);

            // and position menu at last
            var popup = Parent as Popup;
            popup.Placement = PlacementMode.Absolute;
            popup.VerticalOffset = menuPosition.Y;
            popup.HorizontalOffset = menuPosition.X;
        }

        private WinPoint GetMenuStartPoint(Rect trayIconRectangle)
        {
            var menuPosition = new WinPoint(0, 0);
            switch (GetTaskBarLocation())
            {
                case TaskBarLocation.Top:
                    menuPosition.X = trayIconRectangle.X;
                    menuPosition.Y = SystemParameters.WorkArea.Top;
                    break;
                case TaskBarLocation.Left:
                    menuPosition.X = SystemParameters.WorkArea.Left;
                    menuPosition.Y = trayIconRectangle.Y + trayIconRectangle.Height - ActualHeight;
                    break;
                case TaskBarLocation.Right:
                    menuPosition.X = SystemParameters.WorkArea.Right - ActualWidth;
                    menuPosition.Y = trayIconRectangle.Y + trayIconRectangle.Height - ActualHeight;
                    break;
                case TaskBarLocation.Bottom:
                default:
                    menuPosition.X = trayIconRectangle.X;
                    menuPosition.Y = SystemParameters.WorkArea.Height - ActualHeight;
                    break;
            }
            // bound to screen
            menuPosition.X = Math.Min(menuPosition.X, SystemParameters.PrimaryScreenWidth - ActualWidth);
            menuPosition.Y = Math.Min(menuPosition.Y, SystemParameters.PrimaryScreenHeight - ActualHeight);
            menuPosition.X = Math.Max(menuPosition.X, 0);
            menuPosition.Y = Math.Max(menuPosition.Y, 0);

            return menuPosition;
        }

        /// <summary>
        /// Internal "hacky" method to get current tray icon rectangle position. This uses reflection to access TaskBarIcon
        /// NotifyIconData and call Win32 Shell_NotifyIconGetRect. Alas WPF does not provided natively those functionalities.
        /// </summary>
        /// <returns>Primary screen rectangle coordinates of application's icon in notification tray area</returns>
        private Rect GetParentTrayIconPosition()
        {
            // default down right corner position if all should go wrong
            var result = new Rect(SystemParameters.PrimaryScreenWidth - absoluteTrayIconWidth,
                SystemParameters.PrimaryScreenHeight - absoluteTrayIconHeigt, 
                absoluteTrayIconWidth, 
                absoluteTrayIconHeigt);
            try
            {
                var taskBarIconView = System.Windows.Application.Current.MainWindow as TaskBarIconView;

                // use reflection to access private NotifyIconData
                var fieldInfo = typeof(TaskbarIcon).GetField("iconData", ReflectionBindingFlags.Instance | ReflectionBindingFlags.NonPublic);
                var notifyIconData = (NotifyIconData)fieldInfo?.GetValue(taskBarIconView.TaskBarIconInstance);

                // recover from system the notify icon bounding rectangle
                var rect = new RECT();
                var notifyIcon = new NOTIFYICONIDENTIFIER();
                notifyIcon.cbSize = (uint)Marshal.SizeOf(notifyIcon.GetType());
                notifyIcon.hWnd = notifyIconData.WindowHandle;
                notifyIcon.uID = notifyIconData.TaskbarIconId;

                // get form system the bounding rectangle of tray icon
                var shellNotifyIconGetRect = WinUser32.Shell_NotifyIconGetRect(ref notifyIcon, out rect);
                if (shellNotifyIconGetRect != 0)
                {
                    throw new Win32Exception($"Failed to get icon position <{shellNotifyIconGetRect}>.");
                }

                // get primary screen scaling
                double scaleFactor = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11;
                if (scaleFactor < 0.001)
                {
                    scaleFactor = 1;
                }

                result.X = rect.left / scaleFactor;
                result.Y = rect.top / scaleFactor;
                result.Width = (rect.right - rect.left) / scaleFactor;
                result.Height = (rect.bottom - rect.top) / scaleFactor;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to capture tray icon posirtion");
                // all swallowed exceptions here should retsrict to current tray icon position recovery mechanism
            }

            return result;
        }

        /// <summary>
        /// On windows system, user can move the task bar position to any side of teh primary screen.
        /// This method works out current taskbar position
        /// </summary>
        /// <returns>Task bar position on primary screen (where tray icons do appear)</returns>
        private TaskBarLocation GetTaskBarLocation()
        {
            if (SystemParameters.WorkArea.Width == SystemParameters.PrimaryScreenWidth)
            {
                if (SystemParameters.WorkArea.Top > 0)
                {
                    return TaskBarLocation.Top;
                }
                return TaskBarLocation.Bottom;
            }
            if (SystemParameters.WorkArea.Left > 0)
            {
                return TaskBarLocation.Left;
            }
            return TaskBarLocation.Right;
        }
    }
}