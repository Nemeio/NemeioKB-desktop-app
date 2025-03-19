using Microsoft.Extensions.DependencyInjection;

namespace Nemeio.Cli.Application
{
    public interface IPlateformDependencyDelegate
    {
        void RegisterDependencies(ServiceCollection serviceCollection);
    }
}
