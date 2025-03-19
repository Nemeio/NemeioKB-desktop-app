using Microsoft.Extensions.CommandLineUtils;

namespace Nemeio.Cli.Commands.Controllers
{
    internal interface ITestCommandHandlerController
    {
        void RegisterAll(CommandLineApplication commandLinApplication);
    }
}
