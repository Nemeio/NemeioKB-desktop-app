using Microsoft.Extensions.DependencyInjection;
using Nemeio.Tools.Testing.Update.Application.Application;
using Nemeio.Tools.Testing.Update.Application.Windows.Administrator;
using Nemeio.Tools.Testing.Update.Application.Windows.Update.Installer;
using Nemeio.Tools.Testing.Update.Application.Windows.Update.Software;
using Nemeio.Tools.Testing.Update.Core.Administrator;
using Nemeio.Tools.Testing.Update.Core.System;
using Nemeio.Tools.Testing.Update.Core.Update.Installer;
using Nemeio.Tools.Testing.Update.Core.Update.Software;

namespace Nemeio.Tools.Testing.Update.Application.Windows
{
    internal class ApplicationDelegate : IApplicationDelegate
    {
        public void RegisterDependencies(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IAdministratorChecker, WinAdministratorChecker>();
            serviceCollection.AddSingleton<IInstallerExecutor, WinInstallerExecutor>();
            serviceCollection.AddSingleton<ISoftwareExecutor, WinSoftwareExecutor>();
            serviceCollection.AddSingleton<IInstallerCheckerVersion, WinInstallerCheckerVersion>();
            serviceCollection.AddSingleton<ISystemInformation, WinSystemInformation>();
        }
    }
}
