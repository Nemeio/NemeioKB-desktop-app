using System;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Nemeio.Tools.Core.Helpers;
using Nemeio.Tools.Image.ImagePackageBuilder.Applications;
using ApplicationException = Nemeio.Tools.Core.ApplicationException<Nemeio.Tools.Image.ImagePackageBuilder.Applications.ApplicationExitCode>;

namespace Nemeio.Tools.Image.ImagePackageBuilder
{
    internal sealed class ConsoleApplication
    {
        public int Run(ServiceProvider ioc, string[] args)
        {
            var commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: false);
            commandLineApplication.HelpOption("-? | -h | --help");

            var imagesPath = commandLineApplication.Option("-image |--image <image>", "Image path", CommandOptionType.MultipleValue);
            var outputPath = commandLineApplication.Option("-output |--output <output>", "Image package output path", CommandOptionType.SingleValue);
            var compressionType = commandLineApplication.Option("-compression |--compression <compression>", "Image compression format (none, gzip)", CommandOptionType.SingleValue);
            var JSon = commandLineApplication.Option("-json |--json <jsoncontent>", "JSon", CommandOptionType.SingleValue);



            commandLineApplication.OnExecute(async () =>
            {
                var settings = new ApplicationStartupSettings()
                {
                    ImagesPath = imagesPath.Values,
                    OutputPath = outputPath.Value(),
                    CompressionType = compressionType.Value(),
                    JSon = JSon.Value()
                };

                return await Execute(ioc, settings);
            });

            return commandLineApplication.Execute(args);
        }

        private async Task<int> Execute(ServiceProvider ioc, ApplicationStartupSettings settings)
        {
            var application = ioc.GetRequiredService<IImagePackageBuilderApplication>();
            application.Settings = settings;

            ConsoleHelper.WriteLine("Program started ...");
            ConsoleHelper.BreakLine();

            ConsoleHelper.WriteWithColor($"Application started with parameters: {application.Settings}", ConsoleColor.Blue);

            try
            {
                await application.RunAsync();

                ConsoleHelper.WriteSuccess("Image package created successfully!");
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
