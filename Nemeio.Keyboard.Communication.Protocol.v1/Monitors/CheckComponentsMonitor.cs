using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.SetLed;
using Nemeio.Core.Services.TestBench;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class CheckComponentsMonitor : ResponseMonitor, ICheckComponentsMonitor
    {
        private const int ResponsePayloadSize = 1;
        public CheckComponentsMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter)
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            commandExecutor.RegisterNotification(CommandId.CheckComponents, this);
        }

        public CheckComponentResult CheckComponent(byte componentId)
        {
            _logger.LogTrace($"Checking Component {componentId}");

            var command = _commandFactory.CreateCheckComponentsCommand(componentId);
            var responses = ExecuteCommand(command);

            CheckResponsesAndThrowIfNeeded(responses);
            CheckKeyboardErrorCodeAndThrowIfNeeded(responses.First());


            var payload = responses.First().Frame.Payload;

            if (payload.Length < ResponsePayloadSize)
            {
                throw new InvalidOperationException("Component informations are too small / Not available");
            }

            if (payload.Length > ResponsePayloadSize)
            {
                throw new InvalidOperationException("Component informations are too large");
            }


            var binaryReader = new NemeioBinaryReader(payload);
            var result = (KeyboardErrorCode)binaryReader.ReadByte();

            var checkComponentResult = new CheckComponentResult(result);

            return checkComponentResult;
        }
    }
}
