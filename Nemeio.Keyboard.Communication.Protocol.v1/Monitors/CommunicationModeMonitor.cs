using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.CommunicationMode;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class CommunicationModeMonitor : ResponseMonitor, ICommunicationModeMonitor
    {
        public CommunicationModeMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter) 
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            commandExecutor.RegisterNotification(CommandId.SetCommMode, this);
        }

        public void SetCommunicationMode(KeyboardCommunicationMode mode)
        {
            var command = _commandFactory.CreateSetCommunicationModeCommand(mode);
            var responses = ExecuteCommand(command);

            CheckResponsesAndThrowIfNeeded(responses);
            CheckKeyboardErrorCodeAndThrowIfNeeded(responses.First());
        }
    }
}
