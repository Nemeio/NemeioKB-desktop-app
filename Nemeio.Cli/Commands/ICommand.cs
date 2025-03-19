using System.Threading.Tasks;

namespace Nemeio.Cli.Commands
{
    internal interface ICommand
    {
        Task ExecuteAsync();
    }
}
