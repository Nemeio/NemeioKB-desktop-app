using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Configurations.Delete;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class DeleteConfigurationMonitor : ResponseMonitor, IDeleteConfigurationMonitor
    {
        public DeleteConfigurationMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter) 
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            commandExecutor.RegisterNotification(CommandId.DeleteConfig, this);
        }

        public void Delete(LayoutId layoutId)
        {
            if (layoutId == null)
            {
                return;
            }

            _logger.LogTrace($"DeleteConfiguration: Hash=<{layoutId}>");

            var command = _commandFactory.CreateDeleteConfigurationCommand(layoutId);
            var responses = ExecuteCommand(command);

            CheckResponsesAndThrowIfNeeded(responses);
            CheckKeyboardErrorCodeAndThrowIfNeeded(responses.First());
        }
    }
}
