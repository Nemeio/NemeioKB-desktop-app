using System;
using AppKit;
using Nemeio.Core.Managers;
using Nemeio.Platform.Mac.Utils;
using Nemeio.Presentation.Modals;

namespace Nemeio.Mac.Modals
{
    public abstract class MacModalWindow<T> : IModalWindow where T : NSViewController
    {
        protected ILanguageManager _languageManager;
        protected T _nativeModal;

        public bool IsOpen { get; protected set; }

        public event EventHandler<OnClosingModalEventArgs> OnClosing;

        public MacModalWindow(ILanguageManager languageManager)
        {
            _languageManager = languageManager ?? throw new ArgumentNullException(nameof(languageManager));
        }

        public abstract T CreateNativeModal();

        protected void OnClose()
        {
            IsOpen = false;

            _nativeModal = null;
        }

        public void Display()
        {
            DispatchQueueUtils.DispatchAsyncOnMainQueueIfNeeded(() =>
            {
                IsOpen = true;

                _nativeModal = CreateNativeModal();

                var window = new NSWindow();
                window.ContentViewController = _nativeModal;

                var windowController = new NSWindowController(window);
                windowController.ShowWindow(null);
            });
        }

        public void Focus()
        {
            DispatchQueueUtils.DispatchAsyncOnMainQueueIfNeeded(() =>
            {
                var window = _nativeModal.View.Window;

                window.OrderFrontRegardless();
            });
        }

        public void Close()
        {
            DispatchQueueUtils.DispatchAsyncOnMainQueueIfNeeded(() =>
            {
                _nativeModal.DismissController(null);
            });
        }
    }
}
