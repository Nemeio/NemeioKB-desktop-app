using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Battery;
using Nemeio.Core.Keyboard.BatteryElectricalStatus;
using Nemeio.Core.Keyboard.ClearScreen;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.DisplayCheckerBoard;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Services.Batteries;
using Nemeio.Keyboard.Communication.Protocol.v1.Utils;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class DisplayCheckerBoardMonitor : ResponseMonitor, IDisplayCheckerBoardMonitor
    {
        private const int ResponsePayloadSize = 1;

        public DisplayCheckerBoardMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter)
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            commandExecutor.RegisterNotification(CommandId.DisplayCheckerBoard, this);
        }

        public GenericTestBenchResult DisplayCheckerBoard(byte firstColor)
        {
            var displayCheckerBoardCommand = _commandFactory.CreateDisplayCheckerBoardCommand(firstColor);
            var responses = ExecuteCommand(displayCheckerBoardCommand);

            CheckResponsesAndThrowIfNeeded(responses);
            CheckKeyboardErrorCodeAndThrowIfNeeded(responses.First());

            var payload = responses.First().Frame.Payload;

            if (payload.Length < ResponsePayloadSize)
            {
                throw new InvalidOperationException("Response informations are too small / Not available");
            }

            if (payload.Length > ResponsePayloadSize)
            {
                throw new InvalidOperationException("Response informations are too large");
            }


            var binaryReader = new NemeioBinaryReader(payload);
            var result = (KeyboardErrorCode)binaryReader.ReadByte();
            var displayCheckerBoardResult= new GenericTestBenchResult(result);

            return displayCheckerBoardResult;
        }
    }
}
