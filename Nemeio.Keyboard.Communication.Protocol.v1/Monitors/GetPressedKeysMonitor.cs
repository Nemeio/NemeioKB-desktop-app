using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Battery;
using Nemeio.Core.Keyboard.BatteryElectricalStatus;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.PressedKeys;
using Nemeio.Core.Services.Batteries;
using Nemeio.Core.Services.TestBench.PressedKeys;
using Nemeio.Keyboard.Communication.Protocol.v1.Utils;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class GetPressedKeysMonitor : ResponseMonitor, IGetPressedKeysMonitor
    {
        private const int MinResponsePayloadSize = 1;
        private const int MaxResponsePayloadSize = 83;

        public GetPressedKeysMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter)
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            commandExecutor.RegisterNotification(CommandId.GetPressedKeys, this);
        }

        public PressedKeysState GetPressedKeys()
        {
            var getPressedKeysCommand = _commandFactory.CreateGetPressedKeysCommand();
            var responses = ExecuteCommand(getPressedKeysCommand);

            CheckResponsesAndThrowIfNeeded(responses);
            CheckKeyboardErrorCodeAndThrowIfNeeded(responses.First());

            var payload = responses.First().Frame.Payload;

            if (payload.Length < MinResponsePayloadSize)
            {
                throw new InvalidOperationException("Response informations are too small / Not available");
            }

            if (payload.Length > MaxResponsePayloadSize)
            {
                throw new InvalidOperationException("Response informations are too large");
            }

            const int bypassSize = 1;
            var commandPayload = payload.SubArray(bypassSize, payload.Length - bypassSize);
            var pressedKeysStatus = new PressedKeysState(KeyboardErrorCode.Success, new List<ushort>());
            for(int i=0;i <commandPayload.Length;i++)
            {
                var key = commandPayload[i];
                pressedKeysStatus.Keys.Add(key);
            }

            return pressedKeysStatus;
        }
    }
}
