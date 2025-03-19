using System;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Keyboards;
using Nemeio.Cli.Keyboards.Commands;
using ApplicationException = Nemeio.Tools.Core.ApplicationException<Nemeio.Cli.Application.ApplicationExitCode>;

namespace Nemeio.Cli.Commands
{
    internal abstract class CommandHandler : ICommandHandler
    {
        private readonly CommandInfo _info;

        protected readonly ILoggerFactory _loggerFactory;
        protected readonly ILogger _logger;
        protected readonly IOutputWriter _outputWriter;

        public CommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, CommandInfo info) 
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger(typeof(CommandHandler));
            _outputWriter = outputWriter ?? throw new ArgumentNullException(nameof(outputWriter));
            _info = info ?? throw new ArgumentNullException(nameof(info));
        }

        public void Register(CommandLineApplication commandLineApplication)
        {
            commandLineApplication.Command(_info.Name, (command) =>
            {
                command.Description = _info.Description;
                command.HelpOption("-?|-h|--help");
                command.OnExecute(async () =>
                {
                    try
                    {
                        await ExecuteAsync();

                        return (int)ApplicationExitCode.None;
                    }
                    catch (ApplicationException exception)
                    {
                        _logger.LogError(exception, "Program stopped because of an error :");

                        return (int)exception.ExitCode;
                    }
                    //  We want to catch any exception here
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, "Program stopped because of an unknown error.");

                        return (int)ApplicationExitCode.UnknownFailure;
                    }
                });

                RegisterOptions(command);
            });
        }

        public abstract void RegisterOptions(CommandLineApplication application);

        public abstract Task ExecuteAsync();
    }
}
