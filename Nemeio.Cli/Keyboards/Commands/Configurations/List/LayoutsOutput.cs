using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Services.Layouts;
using Newtonsoft.Json;

namespace Nemeio.Cli.Keyboards.Commands.Configurations.List
{
    internal sealed class LayoutsOutput
    {
        [JsonProperty("layouts")]
        public IList<string> Layouts { get; private set; }

        public LayoutsOutput(IList<string> layouts)
        {
            Layouts = layouts;
        }

        public LayoutsOutput(IList<LayoutIdWithHash> ids)
            : this(ids?.Select(x => x.ToString()).ToList()) { }

        public LayoutsOutput(IEnumerable<LayoutIdWithHash> ids)
            : this(ids.ToList()) { }
    }
}
