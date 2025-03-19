using System;
using Nemeio.Core.Transactions;
using Nemeio.Models.Fonts;

namespace Nemeio.Core.DataModels.Configurator
{
    public class Font : IBackupable<Font>
    {
        public string Name { get; private set; }
        public FontSize Size { get; private set; }
        public bool Bold { get; private set; }
        public bool Italic { get; private set; }

        public Font(string name, FontSize size, bool bold, bool italic)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException(nameof(name));
            }

            Name = name;
            Size = size;
            Bold = bold;
            Italic = italic;
        }

        public Font CreateBackup()
        {
            return new Font(
                Name,
                Size,
                Bold,
                Italic
            );
        }
    }
}
