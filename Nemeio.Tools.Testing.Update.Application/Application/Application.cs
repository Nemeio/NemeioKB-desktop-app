using System;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Downloader;
using Nemeio.Core.FileSystem;
using Nemeio.Tools.Testing.Update.Application.Update;
using Nemeio.Tools.Testing.Update.Application.Update.Tester;
using Nemeio.Tools.Testing.Update.Core.Reports.Writers;
using Nemeio.Tools.Testing.Update.Core.Update;
using Nemeio.Tools.Testing.Update.Core.Update.Controller;
using Nemeio.Tools.Testing.Update.Core.Update.Installer;
using Nemeio.Tools.Testing.Update.Core.Update.Tester;
using Nemeio.Tools.Testing.Update.Core.Update.Tester.Exceptions;
using Nemeio.Tools.Testing.Update.Core.Update.Tester.Settings;

namespace Nemeio.Tools.Testing.Update.Application.Application
{
    public class Application
    {
        private CommandOption _settingsFilePath;

        private readonly ServiceCollection _serviceCollection;
        private readonly IApplicationDelegate _applicationDelegate;

        public ServiceProvider IoC { get; private set; }

        public Application(IApplicationDelegate applicationDelegate)
        {
            _serviceCollection = new ServiceCollection();
            _applicationDelegate = applicationDelegate;

            Init();
        }

        public int Execute(string[] arguments)
        {
            var commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: false);
            commandLineApplication.HelpOption("-? | -h | --help");
            commandLineApplication.OnExecute(RunAsync);

            _settingsFilePath = commandLineApplication.Option("-settings|--settings", "Settings of application", CommandOptionType.SingleValue);

            return commandLineApplication.Execute(arguments);
        }

        private void Init()
        {
            _applicationDelegate.RegisterDependencies(_serviceCollection);

            //  Register common dependencies here
            _serviceCollection.AddSingleton<ILoggerFactory>(new LoggerFactory());
            _serviceCollection.AddSingleton<IFileSystem, FileSystem>();
            _serviceCollection.AddSingleton<IFileDownloader, FileDownloader>();
            _serviceCollection.AddSingleton<ITestSettingsParser, TestSettingsParser>();
            _serviceCollection.AddSingleton<ITestSettingsLoader, TestSettingsLoader>();
            _serviceCollection.AddSingleton<IInstallerRepository, InstallerRepository>();
            _serviceCollection.AddSingleton<IReportWriterFactory, ReportWriterFactory>();
            _serviceCollection.AddSingleton<IUpdateTesterFactory, UpdateTesterFactory>();
            _serviceCollection.AddSingleton<IUpdateTestController, UpdateTestController>();

            IoC = _serviceCollection.BuildServiceProvider();
        }

        private async Task<int> RunAsync()
        {
            var updateController = IoC.GetRequiredService<IUpdateTestController>();

            try
            {
                await updateController.StartTestsAsync(_settingsFilePath.Value());

                return (int)ApplicationExitCode.Success;
            }
            catch (MissingSettingsFileException)
            {
                return (int)ApplicationExitCode.MissingSettingsFile;
            }
            catch (MissingAdministratorRightException)
            {
                return (int)ApplicationExitCode.MissingAdministratorRight;
            }
            catch (Exception)
            {
                return (int)ApplicationExitCode.FatalError;
            }
        }
    }
}
