using System.Collections.Generic;

namespace Nemeio.Core.JsonModels
{
    public class KeyboardLayout
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public int Default { get; set; }

        public List<DeadKeyModel> DeadKeys { get; set; }

        public List<KeyModel> Keys { get; set; }
    }
}
