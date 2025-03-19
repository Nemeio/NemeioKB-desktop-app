using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Configurations.Apply;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class ApplyConfigurationMonitor : ResponseMonitor, IApplyConfigurationMonitor
    {
        public ApplyConfigurationMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter) 
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            commandExecutor.RegisterNotification(CommandId.ApplyConfig, this);
        }

        public void Apply(LayoutId id)
        {
            if (id == null)
            {
                return;
            }

            var command = _commandFactory.CreateApplyConfigurationCommand(id);
            var responses = ExecuteCommand(command);

            CheckResponsesAndThrowIfNeeded(responses);
            CheckKeyboardErrorCodeAndThrowIfNeeded(responses.First());
        }
    }
}
