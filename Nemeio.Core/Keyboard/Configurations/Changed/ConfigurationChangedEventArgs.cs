using System;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Keyboard.Configurations.Changed
{
    public class ConfigurationChangedEventArgs
    {
        public LayoutId Configuration { get; private set; }

        public ConfigurationChangedEventArgs(LayoutId id)
        {
            Configuration = id ?? throw new ArgumentNullException(nameof(id));
        }
    }
}
