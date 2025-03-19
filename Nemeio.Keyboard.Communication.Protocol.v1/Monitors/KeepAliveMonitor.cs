using Microsoft.Extensions.Logging;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.KeepAlive;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class KeepAliveMonitor : ResponseMonitor, IKeepAliveMonitor
    {
        public KeepAliveMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter) 
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter) 
        {
            commandExecutor.RegisterNotification(CommandId.KeepAlive, this);
        }

        public void Ping()
        {
            var keepAliveCommand = _commandFactory.CreateKeepAliveCommand();
            var responses = ExecuteCommand(keepAliveCommand);

            CheckResponsesAndThrowIfNeeded(responses);
        }
    }
}
