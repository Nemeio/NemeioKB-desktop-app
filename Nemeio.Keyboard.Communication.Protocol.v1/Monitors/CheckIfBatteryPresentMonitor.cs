using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.CheckIfBatteryPresent;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Services.TestBench;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class CheckIfBatteryPresentMonitor : ResponseMonitor, ICheckIfBatteryPresentMonitor
    {

        private const int ResponsePayloadSize = 2;

        public CheckIfBatteryPresentMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter)
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            commandExecutor.RegisterNotification(CommandId.CheckIfBatteryPresent, this);
        }

        public CheckIfBatteryPresentResult CheckIfBatteryPresent()
        {
            _logger.LogTrace($"Checking Battery presence...");

            var command = _commandFactory.CreateCheckIfBatteryPresentCommand();
            var responses = ExecuteCommand(command);

            CheckResponsesAndThrowIfNeeded(responses);
            CheckKeyboardErrorCodeAndThrowIfNeeded(responses.First());

            var payload = responses.First().Frame.Payload;

            if (payload.Length < ResponsePayloadSize)
            {
                throw new InvalidOperationException("CheckBattery information are too small / Not available");
            }

            if (payload.Length > ResponsePayloadSize)
            {
                throw new InvalidOperationException("CheckBattery information are too large");
            }


            const int bypassSize = 1;
            var commandPayload = payload.SubArray(bypassSize, ResponsePayloadSize - bypassSize);

            var binaryReader = new NemeioBinaryReader(commandPayload);
            var present = binaryReader.ReadByte() != 0;
            var result = new CheckIfBatteryPresentResult(KeyboardErrorCode.Success, present);

            return result;

        }
    }
}
