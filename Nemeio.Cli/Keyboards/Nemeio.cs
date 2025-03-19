using System;
using Nemeio.Core.Keyboard.Communication.Commands;

namespace Nemeio.Cli.Keyboards
{
    internal sealed class Nemeio
    {
        public Core.Keyboard.Keyboard Keyboard { get; private set; }
        public IKeyboardCommandExecutor CommandExecutor { get; private set; }
        public bool IsDisconnected { get; set; }

        public event EventHandler OnDisconnected;

        public Nemeio(Core.Keyboard.Keyboard keyboard, IKeyboardCommandExecutor commandExecutor)
        {
            Keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
            CommandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        }

        public void Disconnected() => OnDisconnected?.Invoke(this, EventArgs.Empty);
    }
}
