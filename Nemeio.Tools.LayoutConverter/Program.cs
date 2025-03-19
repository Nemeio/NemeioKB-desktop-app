using System;
using Microsoft.Extensions.CommandLineUtils;
using Nemeio.Tools.LayoutConverter.Converters;
using Nemeio.Tools.LayoutConverter.Exceptions;
using Nemeio.Tools.LayoutConverter.Factories;
using Nemeio.Tools.LayoutConverter.Models;
using Nemeio.Tools.LayoutConverter.Providers;

namespace Nemeio.Tools.LayoutConverter
{
    class Program
    {
        private static int OkExitCode = 0;

        static void Main(string[] args)
        {
            var commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: false);
            commandLineApplication.HelpOption("-? | -h | --help");

            var folderSelection = commandLineApplication.Option("-folder |--folder <folder>", "The images folder", CommandOptionType.SingleValue);
            var layoutId = commandLineApplication.Option("-id |--id <id>", "The layout id", CommandOptionType.SingleValue);
            var format = commandLineApplication.Option("-format |--format <format>", "The image format to use (1bpp or 2bpp)", CommandOptionType.SingleValue);
            var imageType = commandLineApplication.Option("-type |--type <type>", "The image type (classic, gray, hide or bold)", CommandOptionType.SingleValue);
            var debug = commandLineApplication.Option("-debug |--debug", "Activate debug mode", CommandOptionType.NoValue);

            commandLineApplication.OnExecute(() =>
            {
                Run(layoutId.Value(), folderSelection.Value(), format.Value(), imageType.Value(), debug.HasValue());

                return OkExitCode;
            });

            commandLineApplication.Execute(args);
        }

        static void Run(string layoutId, string folderPath, string format, string imgType, bool debug)
        {
            Console.WriteLine("Welcome on Nemeio layout converter tool!");

            try
            {
                var imageType = new ImageTypeFactory().CreateImageType(imgType);
                var pathProvider = new PathProvider();
                var imageInformations = new ImageConversionInformation(layoutId, folderPath, format, imageType, debug);
                var layoutConverter = new LayoutImageConverter(imageInformations, new AugmentedImageFileProvider(imageType), pathProvider);

                layoutConverter.CreateWallpaper();

                Console.WriteLine("✅ Your wallpaper has been exported successfully");
            }
            catch (ToolException exception) when (exception is MissingRequirementException missingException)
            {
                WriteException(exception);

                foreach (var error in missingException.Errors)
                {
                    WriteError($"{error.Description} for file <{error.FilePath}>");
                }
            }
            catch (ToolException exception)
            {
                WriteException(exception);
            }
            catch (Exception exception)
            {
                WriteException(exception);
            }
        }
        static void WriteException(Exception exception) => WriteError($"{exception.Message}");

        static void WriteException(ToolException exception) => WriteError($"[{exception.ErrorCode}] {exception.AdditionalInformation}");

        static void WriteError(string message) => Console.WriteLine($"❌ Oops! {message}");
    }
}
