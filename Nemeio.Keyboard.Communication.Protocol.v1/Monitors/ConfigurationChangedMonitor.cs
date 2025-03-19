using System;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Configurations.Changed;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class ConfigurationChangedMonitor : NotificationMonitor, IConfigurationChangedMonitor
    {
        public event EventHandler<ConfigurationChangedEventArgs> ConfigurationChanged;

        public ConfigurationChangedMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter) 
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            commandExecutor.RegisterNotification(CommandId.ConfigChanged, this);
        }

        public override void OnReceiveNotification(KeyboardResponse response)
        {
            var id= new LayoutId(response.Frame.Payload);

            ConfigurationChanged?.Invoke(this, new ConfigurationChangedEventArgs(id));
        }
    }
}
