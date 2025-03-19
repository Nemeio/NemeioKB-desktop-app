using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.ExitFunctionalTests;
using Nemeio.Core.Services.Batteries;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class ExitFunctionalTestsMonitor : ResponseMonitor, IExitFunctionalTestsMonitor
    {
        private const int ResponsePayloadSize = 1;
        public ExitFunctionalTestsMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter)
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            commandExecutor.RegisterNotification(CommandId.ExitFunctionalTests, this);
        }

        public GenericTestBenchResult ValidateFunctionalTest(byte validationState)
        {
            _logger.LogTrace($"Validating functional tests");

            var command = _commandFactory.CreateExitFunctionalTestsCommand(validationState);
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
