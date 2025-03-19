using System;

namespace Nemeio.Cli.Commands
{
    internal sealed class CommandInfo
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public CommandInfo(string name, string description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }
    }
}
