using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.FactoryReset;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class FactoryResetMonitor : ResponseMonitor, IFactoryResetMonitor
    {
        public FactoryResetMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter) 
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter) { }

        public void AskFactoryReset()
        {
            var command = _commandFactory.CreateFactoryResetCommand();

            var responses = ExecuteCommand(command);

            CheckResponsesAndThrowIfNeeded(responses);
            CheckKeyboardErrorCodeAndThrowIfNeeded(responses.First());
        }
    }
}
