using Microsoft.Extensions.CommandLineUtils;

namespace Nemeio.Cli.Commands
{
    internal interface ICommandHandler
    {
        void Register(CommandLineApplication commandLineApplication);
    }
}
