using System;
using System.Threading.Tasks;
using System.Windows;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Layouts.Active;
using Nemeio.Presentation.Modals;
using Nemeio.Wpf.UserControls;

namespace Nemeio.Wpf.Modals
{
    public sealed class WinRemovedByHidModal : IModalWindow
    {
        private CloseWindow _closeWindow;
        private readonly IActiveLayoutChangeHandler _activeLayoutChangeHandler;
        private readonly IKeyboardController _keyboardController;

        public WinRemovedByHidModal(IActiveLayoutChangeHandler activeLayoutChangeHandler, IKeyboardController keyboardController)
        {
            _activeLayoutChangeHandler = activeLayoutChangeHandler ?? throw new ArgumentNullException(nameof(activeLayoutChangeHandler));
            _keyboardController = keyboardController ?? throw new ArgumentNullException(nameof(keyboardController));
        }

        public bool IsOpen { get; private set; }

        public event EventHandler<OnClosingModalEventArgs> OnClosing;

        public void Display()
        {
            Application.Current.Dispatcher.Invoke(async delegate
            {
                IsOpen = true;

                _closeWindow = new CloseWindow();

                // get current menu position on screen
                var topLeftCorner = GetBaseWindowTopLeftCorner();

                // prevent window truncation or screen overlap
                double maxLeftPosition = SystemParameters.PrimaryScreenWidth - _closeWindow.Width;
                if (topLeftCorner.X > maxLeftPosition)
                {
                    topLeftCorner.X = maxLeftPosition;
                }

                // set to position closWindow
                _closeWindow.Left = topLeftCorner.X;
                _closeWindow.Top = topLeftCorner.Y;

                // and show up
                var result = _closeWindow.ShowDialog();

                _closeWindow = null;

                if (result.HasValue && result.Value)
                {
                    await _activeLayoutChangeHandler.RequestApplicationShutdownAsync(_keyboardController.Nemeio);

                    Application.Current.Shutdown();
                }

                OnClosing?.Invoke(this, new OnClosingModalEventArgs(this));

                IsOpen = false;
            });
        }

        public void Focus()
        {
            if (_closeWindow != null)
            {
                _closeWindow.Focus();
            }
        }

        private Point GetBaseWindowTopLeftCorner()
        {
            // by default center to screen if no valid positional hint
            var result = new Point(SystemParameters.PrimaryScreenWidth / 2, SystemParameters.PrimaryScreenHeight / 2);

            // get grasp of current control positioning
            var taskBarIconView = Application.Current.MainWindow as TaskBarIconView;
            if (taskBarIconView == null ||
                 taskBarIconView.TaskBarIconInstance.TrayPopup == null)
            {
                return result;
            }
            var taskBarIconMenuView = taskBarIconView.TaskBarIconInstance.TrayPopup;

            // get current menu position on screen
            var topLeftCorner = taskBarIconMenuView.PointToScreen(new Point(0, 0));

            // get primary screen scaling
            double scaleFactor = PresentationSource.FromVisual(taskBarIconMenuView).CompositionTarget.TransformToDevice.M11;
            if (scaleFactor < 0.001)
            {
                scaleFactor = 1;
            }
            result.X = topLeftCorner.X / scaleFactor;
            result.Y = topLeftCorner.Y / scaleFactor;
            return result;
        }

        public void Close()
        {
            _closeWindow.Close();

            OnClosing?.Invoke(this, new OnClosingModalEventArgs(this));

            IsOpen = false;
        }
    }
}
