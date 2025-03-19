using System;
using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Keyboards;

namespace Nemeio.Cli.Commands.Controllers
{
    internal sealed class CommandHandlerController : ICommandHandlerController
    {
        private readonly ILogger _logger;
        private readonly IList<ICommandHandler> _commandHandlers;

        public CommandHandlerController(ILoggerFactory loggerFactory, IList<ICommandHandler> commandHandlers)
        {
            _logger = loggerFactory.CreateLogger<CommandHandlerController>();
            _commandHandlers = commandHandlers ?? throw new ArgumentNullException(nameof(commandHandlers));
        }

        public void RegisterAll(CommandLineApplication commandLinApplication)
        {
            foreach (var handler in _commandHandlers)
            {
                handler.Register(commandLinApplication);
            }
        }
    }
}
