using System;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.SetFrontLightPower;
using Nemeio.Core.Keyboard.SetProvisionning;
using Nemeio.Core.Keyboard.TestBenchId.Set;
using Nemeio.Core.Services.Batteries;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class SetProvisionningMonitor : ResponseMonitor, ISetProvisionningMonitor
    {
        private const int ResponsePayloadSize = 1;
        public SetProvisionningMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter)
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            commandExecutor.RegisterNotification(CommandId.SetProvisionning, this);
        }

        public GenericTestBenchResult SetProvisionning(string serial, PhysicalAddress mac)
        {
            _logger.LogTrace($"Setting Provisionning to Serial:'{serial}' and Mac:'{mac}'");

            var command = _commandFactory.CreateSetProvisionningCommand(serial, mac);
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
