using Nemeio.Cli.Commands;

namespace Nemeio.Cli.Commands.Controllers
{
    internal interface ICommandHandlerControllerBuilder
    {
        ICommandHandlerController BuildOrGet();
    }
}
