using System.Collections.Generic;
using System.Linq;

namespace Nemeio.Core.JsonModels
{
    public class KeyModel
    {
        public int Index { get; set; }

        public string Icon { get; set; }

        public List<string> Actions { get; set; }

        public List<CustomActionModel> CustomActions { get; set; }

        public override string ToString() => CustomActions?
            .Select(s => s.ToString())
            .Aggregate((x, y) => $"{x}\n{y}\n");
    }
}
