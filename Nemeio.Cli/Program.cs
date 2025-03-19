using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Application.Format;
using Nemeio.Cli.Commands.Controllers;
using Nemeio.Cli.Commands.Handlers.Keyboards;
using Nemeio.Cli.Keyboards;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Cli.Keyboards.Commands.Configurations.Add;
using Nemeio.Cli.Keyboards.Commands.Configurations.Apply;
using Nemeio.Cli.Keyboards.Commands.Configurations.Change;
using Nemeio.Cli.Keyboards.Commands.Configurations.Delete;
using Nemeio.Cli.Keyboards.Commands.Configurations.List;
using Nemeio.Cli.Keyboards.Commands.Crashes;
using Nemeio.Cli.Keyboards.Commands.FactoryReset;
using Nemeio.Cli.Keyboards.Commands.Parameters.Get;
using Nemeio.Cli.Keyboards.Commands.Parameters.Set;
using Nemeio.Cli.Keyboards.Commands.TestBench.BatteryElectricalStatus;
using Nemeio.Cli.Keyboards.Commands.TestBench.CheckComponents;
using Nemeio.Cli.Keyboards.Commands.TestBench.ClearScreen;
using Nemeio.Cli.Keyboards.Commands.TestBench.DisplayCheckerBoard;
using Nemeio.Cli.Keyboards.Commands.TestBench.ElectricalTests;
using Nemeio.Cli.Keyboards.Commands.TestBench.FrontLightPower;
using Nemeio.Cli.Keyboards.Commands.TestBench.FunctionalTests;
using Nemeio.Cli.Keyboards.Commands.TestBench.Led;
using Nemeio.Cli.Keyboards.Commands.TestBench.PressedKeys;
using Nemeio.Cli.Keyboards.Commands.TestBench.SetProvisionning;
using Nemeio.Cli.Keyboards.Commands.TestBench.SetAdvertising;
using Nemeio.Cli.Keyboards.Commands.TestBench.TestBenchId.Get;
using Nemeio.Cli.Keyboards.Commands.TestBench.TestBenchId.Set;
using Nemeio.Cli.Keyboards.Commands.Update;
using Nemeio.Cli.Keyboards.Commands.Version;
using Nemeio.Cli.Package.Update.Commands.Read;
using Nemeio.Core;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Core.Keyboard.Communication.Utils;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Tools;
using Nemeio.Core.Tools.Retry;
using Nemeio.Keyboard.Communication.Monitors;
using Nemeio.Keyboard.Communication.Protocol.v1.Commands;
using Nemeio.Keyboard.Communication.Tools.Frames;
using Nemeio.Keyboard.Communication.Tools.Utils;
using Nemeio.Keyboard.Tools.Utils;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Nemeio.Cli.Keyboards.Commands.TestBench.CheckIfBatteryPresent;
using Nemeio.Core.Services.Batteries;

namespace Nemeio.Cli
{
    public static class Program
    {
        public static int Main(IPlateformDependencyDelegate platformDelegate, string[] args)
        {


            if (platformDelegate == null)
            {
                return (int)ApplicationExitCode.UnknownFailure;
            }

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(CreateLoggerFactory());
            platformDelegate.RegisterDependencies(serviceCollection);

            serviceCollection.AddSingleton<IFrameParser, FrameParser>();
            serviceCollection.AddSingleton<ICommandLineExecutor, StandardCommandLineExecutor>();
            serviceCollection.AddSingleton<IFileSystem, FileSystem>();
            serviceCollection.AddSingleton<IRetryHandler, RetryHandler>();
            serviceCollection.AddSingleton<IOutputFormatter, JsonOutputFormatter>();
            serviceCollection.AddSingleton<IOutputWriter, ConsoleOutputWriter>();
            serviceCollection.AddSingleton<IMonitorFactory, MonitorFactory>();
            serviceCollection.AddSingleton<IKeyboardAwaiterFactory, KeyboardAwaiterFactory>();
            serviceCollection.AddSingleton<IKeyboardProvider, KeyboardProvider>();
            serviceCollection.AddSingleton<IKeyboardVersionParser, KeyboardVersionParser>();
            serviceCollection.AddSingleton<IKeyboardSelector, AlwaysFirstKeyboardSelector>();
            serviceCollection.AddSingleton<IKeyboardErrorConverter, KeyboardErrorConverter>();
            serviceCollection.AddSingleton<IKeyboardCommandFactory, KeyboardCommandFactory>();


            if (args.Any("-t".Contains))
            {
                serviceCollection.AddSingleton<ISetLedCommandHandler, SetLedCommandHandler>();
                serviceCollection.AddSingleton<IExitElectricalTestsCommandHandler, ExitElectricalTestsCommandHandler>();
                serviceCollection.AddSingleton<IGetBatteryElectricalStatusCommandHandler, GetBatteryElectricalStatusCommandHandler>();
                serviceCollection.AddSingleton<ISetFrontLightPowerCommandHandler, SetFrontLightPowerCommandHandler>();
                serviceCollection.AddSingleton<ICheckComponentsCommandHandler, CheckComponentsCommandHandler>();
                serviceCollection.AddSingleton<IClearScreenCommandHandler, ClearScreenCommandHandler>();
                serviceCollection.AddSingleton<ISetTestBenchIdCommandHandler, SetTestBenchIdCommandHandler>();
                serviceCollection.AddSingleton<IGetTestBenchIdCommandHandler, GetTestBenchIdCommandHandler>();
                serviceCollection.AddSingleton<ISetProvisionningCommandHandler, SetProvisionningCommandHandler>();
                serviceCollection.AddSingleton<ISetAdvertisingCommandHandler, SetAdvertisingCommandHandler>();
                serviceCollection.AddSingleton<IExitFunctionalTestsCommandHandler, ExitFunctionalTestsCommandHandler>();
                serviceCollection.AddSingleton<IDisplayCheckerBoardCommandHandler, DisplayCheckerBoardCommandHandler>();
                serviceCollection.AddSingleton<IGetPressedKeysCommandHandler, GetPressedKeysCommandHandler>();
                serviceCollection.AddSingleton<ICheckIfBatteryPresentCommandHandler, CheckIfBatteryPresentCommandHandler>();
                serviceCollection.AddSingleton<ITestCommandHandlerControllerBuilder, TestCommandHandlerControllerBuilder>();
            }
            else
            {
                serviceCollection.AddSingleton<IUpdateCommandHandler, UpdateCommandHandler>();
                serviceCollection.AddSingleton<IVersionCommandHandler, VersionCommandHandler>();
                serviceCollection.AddSingleton<IFactoryResetCommandHandler, FactoryResetCommandHandler>();
                serviceCollection.AddSingleton<IGetParametersCommandHandler, GetParametersCommandHandler>();
                serviceCollection.AddSingleton<ISetParameterCommandHandler, SetParameterCommandHandler>();
                serviceCollection.AddSingleton<IDeleteLayoutCommandHandler, DeleteLayoutCommandHandler>();
                serviceCollection.AddSingleton<IListLayoutCommandHandler, ListLayoutCommandHandler>();
                serviceCollection.AddSingleton<IApplyLayoutCommandHandler, ApplyLayoutCommandHandler>();
                serviceCollection.AddSingleton<IAddLayoutCommandHandler, AddLayoutCommandHandler>();
                serviceCollection.AddSingleton<IChangeLayoutListenerCommandHandler, ChangeLayoutListenerCommandHandler>();
                serviceCollection.AddSingleton<IReadUpdatePackageCommandHandler, ReadUpdatePackageCommandHandler>();
                serviceCollection.AddSingleton<ICrashesCommandHandler, CrashesCommandHandler>();
                serviceCollection.AddSingleton<ICommandHandlerControllerBuilder, CommandHandlerControllerBuilder>();
            }
            try
            {

                var ioc = serviceCollection.BuildServiceProvider();
                var consoleApplication = new ConsoleApplication();

                //We need to ensure that params starting with'-t' is placed last after other params
                //WITHOUT CHANGING OTHER PARAMS ORDER
                var orderedArgs = args.Where(x => x != "-t").ToList();
                orderedArgs.AddRange(args.Where(x => x == "-t").ToList());
                int exitCode = -1;
                exitCode = consoleApplication.Run(ioc, orderedArgs.ToArray());
                return exitCode;
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        private static ILoggerFactory CreateLoggerFactory()
        {
            var frameworkSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);

            var loggerFactory = new LoggerFactory();
            var loggerFilename = $"{NemeioConstants.LogFileName}.cli.{NemeioConstants.LogExtension}";

            var serilogger = new LoggerConfiguration()
                .WriteTo.Debug()
                .MinimumLevel.Override("Nemeio.CLI", frameworkSwitch)
                .WriteTo.File(
                    path: Path.Combine(System.IO.Directory.GetCurrentDirectory(), loggerFilename),
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.ffff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day
                )
                .CreateLogger();

            loggerFactory.AddSerilog(serilogger);

            return loggerFactory;
        }
    }
}
