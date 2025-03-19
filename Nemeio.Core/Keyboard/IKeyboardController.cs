using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nemeio.Core.Keyboard.Connection;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Keyboard.State;
using Nemeio.Core.PackageUpdater;

namespace Nemeio.Core.Keyboard
{
    public interface IKeyboardController
    {
        INemeio Nemeio { get; }
        bool Connected { get; }

        event EventHandler<KeyboardStatusChangedEventArgs> OnKeyboardConnected;
        event EventHandler<KeyboardStatusChangedEventArgs> OnKeyboardDisconnecting;
        event EventHandler OnKeyboardDisconnected;
        event EventHandler<KeyboardInitializationFailedEventArgs> OnKeyboardInitializationFailed;
        Task RunAsync();
        void RaisePackageUpdaterStateChanged(PackageUpdateState state);
    }
}
