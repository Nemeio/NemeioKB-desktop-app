using Microsoft.Extensions.DependencyInjection;

namespace Nemeio.Tools.Testing.Update.Application.Application
{
    public interface IApplicationDelegate
    {
        void RegisterDependencies(ServiceCollection serviceCollection);
    }
}
