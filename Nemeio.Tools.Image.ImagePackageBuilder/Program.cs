using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Images.Jpeg.Builder;
using Nemeio.Tools.Image.ImagePackageBuilder.Applications;
using Nemeio.Tools.Image.ImagePackageBuilder.Packages.Writer;

[assembly: InternalsVisibleTo("Nemeio.Tools.Image.ImagePackageBuilder.Tests")]
namespace Nemeio.Tools.Image.ImagePackageBuilder
{
    class Program
    {
        static int Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IFileSystem, FileSystem>();
            serviceCollection.AddTransient<IImagePackageWriter, ImagePackageWriter>();
            serviceCollection.AddTransient<IJpegImagePackageBuilder, JpegImagePackageBuilder>();
            serviceCollection.AddSingleton<IImagePackageBuilderApplication, Application>();

            var ioc = serviceCollection.BuildServiceProvider();

            var consoleApplication = new ConsoleApplication();

            return consoleApplication.Run(ioc, args);
        }
    }
}
