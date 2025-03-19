using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.SetFrontLightPower;
using Nemeio.Core.Keyboard.TestBenchId.Get;
using Nemeio.Core.Keyboard.TestBenchId.Set;
using Nemeio.Core.Services.TestBench;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class GetTestBenchIdMonitor : ResponseMonitor, IGetTestBenchIdMonitor
    {
        
        private const int ResponsePayloadSize = 35;
        
        public GetTestBenchIdMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter)
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            commandExecutor.RegisterNotification(CommandId.GetTestBenchId, this);
        }

        public GetTestBenchIdResult GetTestBenchId()
        {
            _logger.LogTrace($"Getting TestBench Id...");

            var command = _commandFactory.CreateGetTestBenchIdCommand();
            var responses = ExecuteCommand(command);

            CheckResponsesAndThrowIfNeeded(responses);
            CheckKeyboardErrorCodeAndThrowIfNeeded(responses.First());

            var payload = responses.First().Frame.Payload;

            if (payload.Length < ResponsePayloadSize)
            {
                throw new InvalidOperationException("TestBenchId information are too small / Not available");
            }

            if (payload.Length > ResponsePayloadSize)
            {
                throw new InvalidOperationException("TestBenchId information are too large");
            }


            const int bypassSize = 1;
            var commandPayload = payload.SubArray(bypassSize, ResponsePayloadSize - bypassSize);

            var binaryReader = new NemeioBinaryReader(commandPayload);
            var result = new GetTestBenchIdResult(KeyboardErrorCode.Success, System.Text.Encoding.ASCII.GetString(binaryReader.ReadByteList(34).ToArray()));

            return result;

        }
    }
}
