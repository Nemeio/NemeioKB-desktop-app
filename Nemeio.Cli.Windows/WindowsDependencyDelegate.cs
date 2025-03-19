using Microsoft.Extensions.DependencyInjection;
using Nemeio.Cli.Application;
using Nemeio.Core.Keyboard.Communication.Watchers;
using Nemeio.Keyboard.Communication.Windows.Watchers;

namespace Nemeio.Cli.Windows
{
    internal sealed class WindowsDependencyDelegate : IPlateformDependencyDelegate
    {
        void IPlateformDependencyDelegate.RegisterDependencies(ServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IKeyboardWatcherFactory, WinKeyboardWatcherFactory>();
        }
    }
}
