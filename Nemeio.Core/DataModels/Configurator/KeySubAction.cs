using System;
using Nemeio.Core.Enums;
using Nemeio.Core.Transactions;

namespace Nemeio.Core.DataModels.Configurator
{
    public class KeySubAction : IBackupable<KeySubAction>
    {
        public string Data { get;  set; }

        public KeyActionType Type { get; private set; }

        public KeySubAction(string data, KeyActionType type)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Type = type;
        }

        public static KeySubAction CreateModifierAction(string modifier) => new KeySubAction(modifier, KeyActionType.Special);

        public bool IsShift() => IsSpecialKey(KeyboardLiterals.Shift);

        public bool IsAltGr() => IsSpecialKey(KeyboardLiterals.AltGr);

        public bool IsCtrl() => IsSpecialKey(KeyboardLiterals.Ctrl);

        public bool IsAlt() => IsSpecialKey(KeyboardLiterals.Alt);

        public bool IsFunction() => IsSpecialKey(KeyboardLiterals.Fn);

        public bool IsAnyModifier() => IsShift() || IsAltGr();

        private bool IsSpecialKey(string key) => Data == key && Type == KeyActionType.Special;

        public KeySubAction CreateBackup()
        {
            var backup = new KeySubAction(Data, Type);

            return backup;
        }
    }
}
