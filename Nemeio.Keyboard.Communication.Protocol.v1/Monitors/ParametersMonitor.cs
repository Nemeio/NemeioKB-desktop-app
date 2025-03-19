using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.Device;
using Nemeio.Core.Exceptions;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class ParametersMonitor : ResponseMonitor, IParametersMonitor
    {
        public IKeyboardParameterParser Parser { get; private set; }

        public ParametersMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter, IKeyboardParameterParser parser) 
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            Parser = parser ?? throw new ArgumentNullException(nameof(parser));

            commandExecutor.RegisterNotification(CommandId.KeyboardParameters, this);
        }

        public KeyboardParameters GetParameters()
        {
            var payload = SendParameterCommand(_commandFactory.CreateGetParametersCommand());

            _logger.LogInformation($"Parameters received: {CoreHelpers.TraceBuffer(payload)}");

            var parameters = Parser.Parse(payload);

            return parameters;
        }

        public void SetParameters(KeyboardParameters parameters) => SendParameterCommand(_commandFactory.CreateSetParametersCommand(parameters, Parser));

        private byte[] SendParameterCommand(IKeyboardCommand command)
        {
            var responses = ExecuteCommand(command);

            CheckResponsesAndThrowIfNeeded(responses);

            var firstPayload = responses.First().Frame.Payload;
            var errorCode = firstPayload[1];

            if (errorCode != (byte)KeyboardErrorCode.Success)
            {
                throw new CoreException(_errorConverter.Convert((KeyboardErrorCode)errorCode));
            }

            return firstPayload;
        }
    }
}
