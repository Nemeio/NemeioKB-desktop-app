using System;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Configurations.Add;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class AddConfigurationMonitor : ResponseMonitor, IAddConfigurationMonitor
    {
        public AddConfigurationMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter)
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            //  FIXME: Manage only configuration on ReceiveResponse
            commandExecutor.RegisterNotification(CommandId.SendData, this);
        }

        public void SendConfiguration(ILayout layout, bool isFactory = false)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            _logger.LogTrace($"SendConfiguration: Id=<{layout.LayoutId}> Hash=<{layout.Hash}>");

            var command = _commandFactory.CreateSendConfigurationCommand(layout, isFactory);
            var responses = ExecuteCommand(command);

            CheckResponsesAndThrowIfNeeded(responses);

            //  Check every frame error
            foreach (var response in responses)
            {
                CheckKeyboardErrorCodeAndThrowIfNeeded(response);
            }
        }
    }
}
