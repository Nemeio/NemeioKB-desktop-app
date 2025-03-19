using Microsoft.Extensions.DependencyInjection;
using Nemeio.Cli.Application;
using Nemeio.Core.Keyboard.Communication.Watchers;
using Nemeio.Keyboard.Communication.Linux.Watchers;

namespace Nemeio.Cli.Linux
{
    internal sealed class LinuxDependencyDelegate : IPlateformDependencyDelegate
    {
        public void RegisterDependencies(ServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IKeyboardWatcherFactory, LinuxKeyboardWatcherFactory>();
        }
    }
}
