using System;

namespace Nemeio.Presentation.Modals
{
    public sealed class OnClosingModalEventArgs
    {
        public IModalWindow ModalWindow { get; private set; }

        public OnClosingModalEventArgs(IModalWindow modalWindow)
        {
            ModalWindow = modalWindow ?? throw new ArgumentNullException(nameof(modalWindow));
        }
    }
}
