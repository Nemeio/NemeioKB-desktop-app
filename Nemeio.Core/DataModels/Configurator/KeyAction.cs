using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Enums;
using Nemeio.Core.Exceptions;
using Nemeio.Core.Layouts.Color;
using Nemeio.Core.Transactions;

namespace Nemeio.Core.DataModels.Configurator
{
    public class KeyAction : IBackupable<KeyAction>
    {
        public const string EmbeddedPrefix = "emb://";
        public const int MaxItemLength = 6;

        private string _display;

        public KeyboardModifier Modifier { get; set; }

        public string Display
        {
            get => _display;
            set
            {
                if (IsEmbeddedResource(value))
                {
                    _display = value;
                }
                else
                {
                    if (value.Length > MaxItemLength)
                    {
                        throw new TooLargeItemException();
                    }
                    else
                    {
                        _display = value;
                    }
                }
            }
        }

        public IList<KeySubAction> Subactions { get; set; }

        public bool IsGrey { get; set; }

        public static bool IsEmbeddedResource(string val) => val.StartsWith(EmbeddedPrefix);

        public KeyAction CreateBackup()
        {
            var backup = new KeyAction()
            {
                Modifier = Modifier,
                Display = Display,
                Subactions = Subactions
                    .Select(subaction => subaction.CreateBackup())
                    .ToList(),
                IsGrey = IsGrey,
            };

            return backup;
        }
    }
}
