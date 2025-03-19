using System;
using System.Windows;
using Nemeio.Presentation.Modals;

namespace Nemeio.Wpf.Modals
{
    public abstract class WinModalWindow<T> : IModalWindow where T : Window
    {
        private T _nativeWindow;

        public bool IsOpen { get; private set; }

        public event EventHandler<OnClosingModalEventArgs> OnClosing;

        public abstract T CreateNativeWindow();

        public void Display()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                IsOpen = true;

                _nativeWindow = CreateNativeWindow();
                _nativeWindow.Topmost = true;
                _nativeWindow.Closed += NativeWindow_Closed;
                _nativeWindow.Show();
            });
        }

        private void NativeWindow_Closed(object sender, EventArgs e)
        {
            _nativeWindow = null;

            OnClosing?.Invoke(this, new OnClosingModalEventArgs(this));

            IsOpen = false;
        }

        public void Focus()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                if (_nativeWindow != null)
                {
                    _nativeWindow.Focus();
                }
            });
        }

        public void Close()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                _nativeWindow?.Close();

                IsOpen = false;

                OnClosing?.Invoke(this, new OnClosingModalEventArgs(this));
            });
        }
    }
}
