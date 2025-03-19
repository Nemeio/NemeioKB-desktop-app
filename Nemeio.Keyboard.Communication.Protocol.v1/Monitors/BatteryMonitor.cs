using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Battery;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Services.Batteries;
using Nemeio.Keyboard.Communication.Protocol.v1.Utils;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class BatteryMonitor : ResponseMonitor, IBatteryMonitor
    {
        private const int ResponsePayloadSize = 15;

        public BatteryMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter) 
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter) 
        {
            commandExecutor.RegisterNotification(CommandId.Battery, this);
        }

        public BatteryInformation AskBattery()
        {
            var batteryCommand = _commandFactory.CreateBatteryCommand();
            var responses = ExecuteCommand(batteryCommand);

            CheckResponsesAndThrowIfNeeded(responses);
            CheckKeyboardErrorCodeAndThrowIfNeeded(responses.First());

            var payload = responses.First().Frame.Payload;

            if (payload.Length < ResponsePayloadSize)
            {
                throw new InvalidOperationException("Battery information are too large");
            }

            const int bypassSize = 1;
            var commandPayload = payload.SubArray(bypassSize, ResponsePayloadSize - bypassSize);

            var binaryReader = new NemeioBinaryReader(commandPayload);
            var batteryInformation = new BatteryInformation(
                new BatteryLevel(binaryReader.ReadByte()),
                binaryReader.ReadUInt16(),
                new BatteryTime(binaryReader.ReadUInt32()),
                new BatteryTime(binaryReader.ReadUInt32()),
                binaryReader.ReadUInt16(),
                binaryReader.ReadByte()
            );

            return batteryInformation;
        }
    }
}
