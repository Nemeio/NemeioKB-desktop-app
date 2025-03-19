using System.Collections.Generic;
using Nemeio.Core.DataModels.Configurator;

namespace Nemeio.Core.Services.Layouts
{
    public class KeyboardLayout
    {
        public byte[] Image { get; set; }

        public IEnumerable<Key> Keys { get; set; }
    }
}
