using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Enums;
using Nemeio.Core.Exceptions;
using Nemeio.Core.Transactions;

namespace Nemeio.Core.DataModels.Configurator
{
    public class Key : IBackupable<Key>, ICloneable
    {
        //  None, Shift, AltGr, Shift + AltGr, Function, Capslock
        public const int MaxNumberOfActionSupported = 6;    

        private IList<KeyAction> _actions;

        public int Index { get; set; }
        public KeyDisposition Disposition { get; set; }
        public Font Font { get; set; }
        public IList<KeyAction> Actions
        {
            get => _actions;
            set
            {
                if (value.Count > MaxNumberOfActionSupported)
                {
                    throw new TooManyActionException();
                }
                else
                {
                    _actions = value;
                }
            }
        }
        public bool Edited { get; set; } = false;

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public Key CreateBackup()
        {
            return new Key()
            {
                Index = Index,
                Disposition = Disposition,
                Font = Font != null ? new Font(Font.Name, Font.Size, Font.Bold, Font.Italic) : null,
                Actions = Actions
                    .Select(action => action.CreateBackup())
                    .ToList(),
                Edited = Edited
            };
        }
    }
}
