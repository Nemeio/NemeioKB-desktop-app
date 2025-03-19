using System;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Nemeio.Tools.Core.Helpers;
using ApplicationException = Nemeio.Tools.Core.ApplicationException<Nemeio.Tools.Firmware.PackageBuilder.Applications.ApplicationExitCode>;

namespace Nemeio.Tools.Firmware.PackageBuilder.Applications
{
    internal class ConsoleApplication
    {
        public int Run(ServiceProvider ioc, string[] args)
        {
            var commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: false);
            commandLineApplication.HelpOption("-? | -h | --help");

            var manifestFilePath = commandLineApplication.Option("-manifest |--manifest <manifest>", "Manifest file path", CommandOptionType.SingleValue);
            var inputFileDirectoryPath = commandLineApplication.Option("-D |--D <D>", "Input file directory path", CommandOptionType.SingleValue);
            var outputFileDirectoryPath = commandLineApplication.Option("-o |--o <o>", "Output file path", CommandOptionType.SingleValue);

            commandLineApplication.OnExecute(async () =>
            {
                var settings = new ApplicationStartupSettings()
                {
                    ManifestFilePath = manifestFilePath.Value(),
                    InputFileDirectoryPath = inputFileDirectoryPath.Value(),
                    OuputFilePath = outputFileDirectoryPath.Value()
                };

                return await Execute(ioc, settings);
            });

            return commandLineApplication.Execute(args);
        }

        private async Task<int> Execute(ServiceProvider ioc, ApplicationStartupSettings settings)
        {
            var application = ioc.GetRequiredService<IPackageBuilderApplication>();
            application.Settings = settings;

            ConsoleHelper.WriteLine("Program started ...");
            ConsoleHelper.BreakLine();

            ConsoleHelper.WriteWithColor($"Application started with parameters: {application.Settings}", ConsoleColor.Blue);

            try
            {
                await application.RunAsync();

                ConsoleHelper.WriteSuccess("Binary package created successfully!");
                ConsoleHelper.BreakLine();

                return (int)ApplicationExitCode.Success;
            }
            catch (ApplicationException exception)
            {
                ConsoleHelper.WriteError("Program stopped because of an error :");
                ConsoleHelper.WriteException(exception);
                ConsoleHelper.BreakLine();

                return (int)exception.ExitCode;
            }
            //  We want to catch any exception here
            catch (Exception exception)
            {
                ConsoleHelper.WriteError("Program stopped because of an unknown error.");
                ConsoleHelper.WriteError("Please contact developers.");
                ConsoleHelper.WriteException(exception);
                ConsoleHelper.BreakLine();

                return (int)ApplicationExitCode.UnknownFailure;
            }
            finally
            {
                ConsoleHelper.WriteLine("Program ended");
                ConsoleHelper.BreakLine();
            }
        }
    }
}
