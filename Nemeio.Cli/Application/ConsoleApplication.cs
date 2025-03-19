using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Nemeio.Cli.Commands.Controllers;
using System.Linq;

namespace Nemeio.Cli.Application
{
    internal class ConsoleApplication
    {
        public int Run(ServiceProvider ioc, string[] args)
        {
            var commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: true);
            try
            {


                if (args.Any("-t".Contains))
                {
                    var testCommandHandlerControllerBuilder = ioc.GetRequiredService<ITestCommandHandlerControllerBuilder>();
                    var testCommandHandlerController = testCommandHandlerControllerBuilder.BuildOrGet();
                    testCommandHandlerController.RegisterAll(commandLineApplication);
                    args = args.Where(x => x != "-t").ToArray();


                }
                else
                {
                    var commandHandlerControllerBuilder = ioc.GetRequiredService<ICommandHandlerControllerBuilder>();
                    var commandHandlerController = commandHandlerControllerBuilder.BuildOrGet();
                    commandHandlerController.RegisterAll(commandLineApplication);
                }

                commandLineApplication.HelpOption("-? | -h | --help");




                var exitCode = commandLineApplication.Execute(args);

                return exitCode;

            }
            catch (System.Exception ex)
            {
                var outputWriter = ioc.GetService<IOutputWriter>();
                outputWriter.WriteLine("add -t to target TestBench methods");
                outputWriter.WriteErrorLine(ex.Message);
                return -1;
            }
        }
    }
}
