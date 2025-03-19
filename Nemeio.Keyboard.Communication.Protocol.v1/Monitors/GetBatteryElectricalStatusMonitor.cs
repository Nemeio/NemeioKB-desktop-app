using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Battery;
using Nemeio.Core.Keyboard.BatteryElectricalStatus;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Services.Batteries;
using Nemeio.Keyboard.Communication.Protocol.v1.Utils;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class GetBatteryElectricalStatusMonitor : ResponseMonitor, IGetBatteryElectricalStatusMonitor
    {
        private const int ResponsePayloadSize = 5;

        public GetBatteryElectricalStatusMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter)
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            commandExecutor.RegisterNotification(CommandId.GetBatteryElectricalStatus, this);
        }

        public BatteryElectricalState GetBatteryElectricalStatus()
        {
            var getBatteryElectricalStatusCommand = _commandFactory.CreateGetBatteryElectricalStateCommand();
            var responses = ExecuteCommand(getBatteryElectricalStatusCommand);

            CheckResponsesAndThrowIfNeeded(responses);
            CheckKeyboardErrorCodeAndThrowIfNeeded(responses.First());

            var payload = responses.First().Frame.Payload;

            if (payload.Length < ResponsePayloadSize)
            {
                throw new InvalidOperationException("Battery information are too small / Not available");
            }

            if (payload.Length > ResponsePayloadSize)
            {
                throw new InvalidOperationException("Battery information are too large");
            }

            const int bypassSize = 1;
            var commandPayload = payload.SubArray(bypassSize, ResponsePayloadSize - bypassSize);

            var binaryReader = new NemeioBinaryReader(commandPayload);
            var batteryElectricalStatus = new BatteryElectricalState(KeyboardErrorCode.Success, BitConverter.ToInt16(binaryReader.ReadByteList(2).ToArray(), 0), BitConverter.ToUInt16(binaryReader.ReadByteList(2).ToArray(), 0));

            return batteryElectricalStatus;
        }
    }
}
