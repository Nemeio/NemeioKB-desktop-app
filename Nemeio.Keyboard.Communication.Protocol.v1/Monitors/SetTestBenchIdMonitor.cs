using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.SetFrontLightPower;
using Nemeio.Core.Keyboard.TestBenchId.Set;
using Nemeio.Core.Services.Batteries;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class SetTestBenchIdMonitor : ResponseMonitor, ISetTestBenchIdMonitor
    {
        private const int ResponsePayloadSize = 1;
        public SetTestBenchIdMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter)
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            commandExecutor.RegisterNotification(CommandId.SetTestBenchId, this);
        }

        public GenericTestBenchResult SetTestBenchId(string testId)
        {
            _logger.LogTrace($"Setting TestBench id to {testId}");

            var command = _commandFactory.CreateSetTestBenchIdCommand(testId);
            var responses = ExecuteCommand(command);

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
            var testBenchresult = new GenericTestBenchResult(result);

            return testBenchresult;
        }
    }
}
