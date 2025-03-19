using Microsoft.Extensions.CommandLineUtils;

namespace Nemeio.Cli.Commands.Controllers
{
    internal interface ICommandHandlerController
    {
        void RegisterAll(CommandLineApplication commandLinApplication);
    }
}
