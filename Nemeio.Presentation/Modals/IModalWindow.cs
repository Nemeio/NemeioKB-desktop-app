using System;

namespace Nemeio.Presentation.Modals
{
    public interface IModalWindow
    {
        bool IsOpen { get; }

        event EventHandler<OnClosingModalEventArgs> OnClosing;

        void Display();
        void Focus();
        void Close();
    }
}
