using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.SerialNumber;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class SerialNumberMonitor : ResponseMonitor, ISerialNumberMonitor
    {
        public SerialNumberMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter) 
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter) 
        {
            commandExecutor.RegisterNotification(CommandId.SerialNumber, this);
        }

        public NemeioSerialNumber AskSerialNumber()
        {
            var serialNumberCommand = _commandFactory.CreateSerialNumberCommand();

            var responses = ExecuteCommand(serialNumberCommand);

            CheckResponsesAndThrowIfNeeded(responses);

            var payload = responses.First().Frame.Payload;
            var serialNumber = new NemeioSerialNumber(payload);

            return serialNumber;
        }
    }
}
