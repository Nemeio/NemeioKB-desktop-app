using System;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.Updates;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class UpdateMonitor : ResponseMonitor, IUpdateMonitor
    {
        public UpdateMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter) 
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            commandExecutor.RegisterNotification(CommandId.SendData, this);
        }

        public void SendFirmware(byte[] firmware)
        {
            //  Update monitor not manage any error code from keyboard.
            //  Keyboard will disconnect himself when it start updating.

            try
            {
                var command = _commandFactory.CreateSendFirmwareCommand(firmware);
                ExecuteCommand(command);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "SendFirmware error");
            }
        }
    }
}
